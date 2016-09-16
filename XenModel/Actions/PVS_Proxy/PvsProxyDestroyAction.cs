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
using System.Diagnostics;
using System.Threading;
using XenAPI;
using System.Linq;

namespace XenAdmin.Actions
{
    public class PvsProxyDestroyAction : AsyncAction
    {
        private PVS_proxy proxy;

        public PvsProxyDestroyAction(PVS_proxy proxy)
            : base(proxy.Connection, "TODO: enabling PVS read-caching")
        {
            this.proxy = proxy;
            this.Session = proxy.Connection.Session;
            this.VM = proxy.VM;

            this.Description = Messages.WAITING;
            SetRBACPermissions();
        }

        private void SetRBACPermissions()
        {
            //AddCommonAPIMethodsToRoleCheck();
            //ApiMethodsToRoleCheck.Add("host.destroy");
            //ApiMethodsToRoleCheck.Add("sr.forget");

            // TODO: what to use here?
        }

        protected override void Run()
        {
            Title = "TODO";
            Description = "TODO: disabling read-caching";

            try
            {
                PVS_proxy.async_destroy(Session, proxy.opaque_ref);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }
    }
}