﻿/* Copyright (c) Citrix Systems Inc. 
 * All rights reserved. 
 * 
 * Redistribution and use in source and binary forms, 
 * with or without modification, are permitted provided 
 * that the following conditions are met: 
 * 
 * *   Redistributions of source code must retain the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer. 
 * *   Redistributions in binary form must reproduce the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer in the documentation and/or other 
 *     materials provided with the distribution. 
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND 
 * CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
 * INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
 * MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR 
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF 
 * SUCH DAMAGE.
 */


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Network;
using XenAdmin.Dialogs;
using XenAPI;
using XenAdmin.Commands;

namespace XenAdmin.TabPages
{
    public partial class PvsPage : BaseTabPage
    {
        private IXenConnection connection;
        private SelectionManager enableSelectionManager;
        private SelectionManager disableSelectionManager;
        
        public PvsPage()
        {
            InitializeComponent();

            enableButton.Command = new EnablePvsReadCachingCommand();
            disableButton.Command = new DisablePvsReadCachingCommand();

            enableSelectionManager = new SelectionManager();
            disableSelectionManager = new SelectionManager();
            
            base.Text = Messages.PVS_TAB_TITLE;
        }

        public IXenConnection Connection
        {
            get
            {
                return connection;
            }
            set
            {
                if (connection != null)
                {
                    connection.Cache.DeregisterBatchCollectionChanged<PVS_site>(PvsSiteBatchCollectionChanged);
                    connection.Cache.DeregisterBatchCollectionChanged<PVS_proxy>(PvsProxyBatchCollectionChanged);
                    connection.Cache.DeregisterBatchCollectionChanged<VM>(PvsProxyBatchCollectionChanged);
                }

                connection = value;

                if (connection != null)
                {
                    connection.Cache.RegisterBatchCollectionChanged<PVS_site>(PvsSiteBatchCollectionChanged);
                    connection.Cache.RegisterBatchCollectionChanged<PVS_proxy>(PvsProxyBatchCollectionChanged);
                    connection.Cache.RegisterBatchCollectionChanged<VM>(PvsProxyBatchCollectionChanged);
                }

                LoadSites();
                LoadVMs();
            }
        }

        private void LoadSites()
        {
            Program.AssertOnEventThread();

            if (!Visible)
                return;

            try
            {
                dataGridViewSites.SuspendLayout();
                dataGridViewSites.Rows.Clear();

                var rowList = new List<DataGridViewRow>();

                foreach (var pvsSite in Connection.Cache.PVS_sites)
                    rowList.Add(NewPvsSiteRow(pvsSite));

                dataGridViewSites.Rows.AddRange(rowList.ToArray());

                if (dataGridViewSites.SelectedRows.Count == 0 && dataGridViewSites.Rows.Count > 0)
                    dataGridViewSites.Rows[0].Selected = true;
            }
            finally
            {
                dataGridViewSites.ResumeLayout();
            }
        }

        private void LoadVMs()
        {
            Program.AssertOnEventThread();

            if (!Visible)
                return;

            try
            {
                dataGridViewVms.SuspendLayout();

                var previousSelection = GetSelectedVMs();
                
                UnregisterVMHandlers();
                dataGridViewVms.Rows.Clear();

                enableSelectionManager.BindTo(enableButton, Program.MainWindow);
                disableSelectionManager.BindTo(disableButton, Program.MainWindow);
               
                //foreach (var pvsProxy in Connection.Cache.PVS_proxies.Where(p => p.VM != null))
                foreach (var vm in Connection.Cache.VMs.Where(vm => vm.is_a_real_vm && vm.Show(Properties.Settings.Default.ShowHiddenVMs)))
                    dataGridViewVms.Rows.Add(NewVmRow(vm));

                if (dataGridViewVms.Rows.Count > 0)
                {
                    if (previousSelection.Any())
                    {
                        UnselectAllVMs(); // Component defaults the first row to selected
                        foreach (var row in dataGridViewVms.Rows.Cast<DataGridViewRow>())
                        {
                            if (previousSelection.Contains((VM)row.Tag))
                            {
                                row.Selected = true;
                            }
                        }
                    }
                    else
                    {
                        dataGridViewVms.Rows[0].Selected = true;
                    }
                }

                dataGridViewVms.SelectionChanged += VmSelectionChanged;
            }
            finally
            {
                dataGridViewVms.ResumeLayout();
            }
        }

        private void UnselectAllVMs()
        {
            foreach (var row in dataGridViewVms.SelectedRows.Cast<DataGridViewRow>())
            {
                row.Selected = false;
            }
        }

        private IList<VM> GetSelectedVMs()
        {
            return dataGridViewVms.SelectedRows.Cast<DataGridViewRow>().Select(row => (VM)row.Tag).ToList();
        }

        private void VmSelectionChanged(object sender, EventArgs e)
        {
            var selectedVMs = GetSelectedVMs().Select(vm => new SelectedItem(vm));
            enableSelectionManager.SetSelection(selectedVMs);
            disableSelectionManager.SetSelection(selectedVMs);
        }

        private DataGridViewRow NewPvsSiteRow(PVS_site pvsSite)
        {
            var siteCell = new DataGridViewTextBoxCell {Value = pvsSite.name};
            var configurationCell = new DataGridViewTextBoxCell
            {
                Value = pvsSite.cache_storage.Count > 0 ? Messages.PVS_CACHE_MEMORY_AND_DISK: Messages.PVS_CACHE_MEMORY_ONLY
            };
            var cacheSrsCell = new DataGridViewTextBoxCell
            {
                Value = string.Join(",  ", Connection.ResolveAll(pvsSite.cache_storage))
            };

            var newRow = new DataGridViewRow { Tag = pvsSite };
            newRow.Cells.AddRange(siteCell, configurationCell, cacheSrsCell);

            return newRow;
        }

        private DataGridViewRow NewVmRow(VM vm)
        {
            System.Diagnostics.Trace.Assert(vm != null);

            var pvsProxy = vm.Connection.Cache.PVS_proxies.FirstOrDefault(p => p.VM.Equals(vm)); // null if no proxy
            
            if (pvsProxy == null)
            {
                return NewVmRowWithNoProxy(vm);
            }
            return NewVmRowWithProxy(vm, pvsProxy);
        }

        private DataGridViewRow NewVmRowWithNoProxy(VM vm)
        {
            var vmCell = new DataGridViewTextBoxCell { Value = vm.Name };
            var cacheEnabledCell = new DataGridViewTextBoxCell { Value = Messages.NO };
            var cachedCell = new DataGridViewTextBoxCell { Value = Messages.NO_VALUE };
            var pvsSiteCell = new DataGridViewTextBoxCell { Value = Messages.NO_VALUE };
            var srCell = new DataGridViewTextBoxCell { Value = Messages.NO_VALUE };

            var newRow = new DataGridViewRow { Tag = vm };
            newRow.Cells.AddRange(vmCell, cacheEnabledCell, cachedCell, pvsSiteCell, srCell);
            vm.PropertyChanged += VmPropertyChanged;

            return newRow;
        }

        private DataGridViewRow NewVmRowWithProxy(VM vm, PVS_proxy pvsProxy)
        {
            System.Diagnostics.Trace.Assert(pvsProxy != null);

            var vmCell = new DataGridViewTextBoxCell { Value = vm.Name };
            var cacheEnabledCell = new DataGridViewTextBoxCell { Value = Messages.YES };

            var cachedCell = new DataGridViewTextBoxCell
            {
                Value = pvsProxy.currently_attached ? Messages.YES : Messages.NO
            };

            var pvsSiteCell = new DataGridViewTextBoxCell { Value = Connection.Resolve(pvsProxy.site) };

            var sr = Connection.Resolve(pvsProxy.cache_SR);
            var srCell = new DataGridViewTextBoxCell { 
                Value = sr != null? sr.ToString() : Messages.NONE_MEMORY_ONLY 
            };

            var newRow = new DataGridViewRow { Tag = vm };
            newRow.Cells.AddRange(vmCell, cacheEnabledCell, cachedCell, pvsSiteCell, srCell);
            vm.PropertyChanged += VmPropertyChanged;

            return newRow;
        }

        private void UnregisterVMHandlers()
        {
            dataGridViewVms.SelectionChanged -= VmSelectionChanged;

            foreach (DataGridViewRow row in dataGridViewVms.Rows)
            {
                var vm = row.Tag as VM;
                System.Diagnostics.Trace.Assert(vm != null);
                vm.PropertyChanged -= VmPropertyChanged;
            }
        }

        private void PvsSiteBatchCollectionChanged(object sender, EventArgs e)
        {
            Program.Invoke(this, LoadSites); 
        }

        private void PvsProxyBatchCollectionChanged(object sender, EventArgs e)
        {
            Program.Invoke(this, LoadVMs);
        }

        private void VmPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "name_label")
                return;

            foreach (DataGridViewRow row in dataGridViewVms.Rows)
            {
                var vm = row.Tag as VM;
                if (vm != null && vm.Equals(sender))
                {
                    row.Cells["columnVM"].Value = vm.Name;
                    break;
                }
            }
        }

        private void ViewPvsSitesButton_Click(object sender, EventArgs e)
        {
            Program.MainWindow.ShowPerConnectionWizard(connection, new PvsSiteDialog(connection));
        }
    }
}
