/*
 * Copyright (c) Citrix Systems, Inc.
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 * 
 *   1) Redistributions of source code must retain the above copyright
 *      notice, this list of conditions and the following disclaimer.
 * 
 *   2) Redistributions in binary form must reproduce the above
 *      copyright notice, this list of conditions and the following
 *      disclaimer in the documentation and/or other materials
 *      provided with the distribution.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
 * FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE
 * COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
 * INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
 * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
 * STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
 * OF THE POSSIBILITY OF SUCH DAMAGE.
 */


using System;
using System.Collections;
using System.Collections.Generic;

using CookComputing.XmlRpc;


namespace XenAPI
{
    /// <summary>
    /// machines serving blocks of data for provisioning VMs
    /// </summary>
    public partial class PVS_site : XenObject<PVS_site>
    {
        public PVS_site()
        {
        }

        public PVS_site(string uuid,
            string name,
            List<XenRef<PVS_cache_storage>> cache_storage,
            List<XenRef<PVS_server>> servers,
            List<XenRef<PVS_proxy>> proxies)
        {
            this.uuid = uuid;
            this.name = name;
            this.cache_storage = cache_storage;
            this.servers = servers;
            this.proxies = proxies;
        }

        /// <summary>
        /// Creates a new PVS_site from a Proxy_PVS_site.
        /// </summary>
        /// <param name="proxy"></param>
        public PVS_site(Proxy_PVS_site proxy)
        {
            this.UpdateFromProxy(proxy);
        }

        public override void UpdateFrom(PVS_site update)
        {
            uuid = update.uuid;
            name = update.name;
            cache_storage = update.cache_storage;
            servers = update.servers;
            proxies = update.proxies;
        }

        internal void UpdateFromProxy(Proxy_PVS_site proxy)
        {
            uuid = proxy.uuid == null ? null : (string)proxy.uuid;
            name = proxy.name == null ? null : (string)proxy.name;
            cache_storage = proxy.cache_storage == null ? null : XenRef<PVS_cache_storage>.Create(proxy.cache_storage);
            servers = proxy.servers == null ? null : XenRef<PVS_server>.Create(proxy.servers);
            proxies = proxy.proxies == null ? null : XenRef<PVS_proxy>.Create(proxy.proxies);
        }

        public Proxy_PVS_site ToProxy()
        {
            Proxy_PVS_site result_ = new Proxy_PVS_site();
            result_.uuid = (uuid != null) ? uuid : "";
            result_.name = (name != null) ? name : "";
            result_.cache_storage = (cache_storage != null) ? Helper.RefListToStringArray(cache_storage) : new string[] {};
            result_.servers = (servers != null) ? Helper.RefListToStringArray(servers) : new string[] {};
            result_.proxies = (proxies != null) ? Helper.RefListToStringArray(proxies) : new string[] {};
            return result_;
        }

        /// <summary>
        /// Creates a new PVS_site from a Hashtable.
        /// </summary>
        /// <param name="table"></param>
        public PVS_site(Hashtable table)
        {
            uuid = Marshalling.ParseString(table, "uuid");
            name = Marshalling.ParseString(table, "name");
            cache_storage = Marshalling.ParseSetRef<PVS_cache_storage>(table, "cache_storage");
            servers = Marshalling.ParseSetRef<PVS_server>(table, "servers");
            proxies = Marshalling.ParseSetRef<PVS_proxy>(table, "proxies");
        }

        public bool DeepEquals(PVS_site other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._name, other._name) &&
                Helper.AreEqual2(this._cache_storage, other._cache_storage) &&
                Helper.AreEqual2(this._servers, other._servers) &&
                Helper.AreEqual2(this._proxies, other._proxies);
        }

        public override string SaveChanges(Session session, string opaqueRef, PVS_site server)
        {
            if (opaqueRef == null)
            {
                System.Diagnostics.Debug.Assert(false, "Cannot create instances of this type on the server");
                return "";
            }
            else
            {
                if (!Helper.AreEqual2(_name, server._name))
                {
                    PVS_site.set_name(session, opaqueRef, _name);
                }

                return null;
            }
        }
        /// <summary>
        /// Get a record containing the current state of the given PVS_site.
        /// Experimental. First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_site">The opaque_ref of the given pvs_site</param>
        public static PVS_site get_record(Session session, string _pvs_site)
        {
            return new PVS_site((Proxy_PVS_site)session.proxy.pvs_site_get_record(session.uuid, (_pvs_site != null) ? _pvs_site : "").parse());
        }

        /// <summary>
        /// Get a reference to the PVS_site instance with the specified UUID.
        /// Experimental. First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        public static XenRef<PVS_site> get_by_uuid(Session session, string _uuid)
        {
            return XenRef<PVS_site>.Create(session.proxy.pvs_site_get_by_uuid(session.uuid, (_uuid != null) ? _uuid : "").parse());
        }

        /// <summary>
        /// Get the uuid field of the given PVS_site.
        /// Experimental. First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_site">The opaque_ref of the given pvs_site</param>
        public static string get_uuid(Session session, string _pvs_site)
        {
            return (string)session.proxy.pvs_site_get_uuid(session.uuid, (_pvs_site != null) ? _pvs_site : "").parse();
        }

        /// <summary>
        /// Get the name field of the given PVS_site.
        /// Experimental. First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_site">The opaque_ref of the given pvs_site</param>
        public static string get_name(Session session, string _pvs_site)
        {
            return (string)session.proxy.pvs_site_get_name(session.uuid, (_pvs_site != null) ? _pvs_site : "").parse();
        }

        /// <summary>
        /// Get the cache_storage field of the given PVS_site.
        /// Experimental. First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_site">The opaque_ref of the given pvs_site</param>
        public static List<XenRef<PVS_cache_storage>> get_cache_storage(Session session, string _pvs_site)
        {
            return XenRef<PVS_cache_storage>.Create(session.proxy.pvs_site_get_cache_storage(session.uuid, (_pvs_site != null) ? _pvs_site : "").parse());
        }

        /// <summary>
        /// Get the servers field of the given PVS_site.
        /// Experimental. First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_site">The opaque_ref of the given pvs_site</param>
        public static List<XenRef<PVS_server>> get_servers(Session session, string _pvs_site)
        {
            return XenRef<PVS_server>.Create(session.proxy.pvs_site_get_servers(session.uuid, (_pvs_site != null) ? _pvs_site : "").parse());
        }

        /// <summary>
        /// Get the proxies field of the given PVS_site.
        /// Experimental. First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_site">The opaque_ref of the given pvs_site</param>
        public static List<XenRef<PVS_proxy>> get_proxies(Session session, string _pvs_site)
        {
            return XenRef<PVS_proxy>.Create(session.proxy.pvs_site_get_proxies(session.uuid, (_pvs_site != null) ? _pvs_site : "").parse());
        }

        /// <summary>
        /// Introduce new PVS site
        /// Experimental. First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_name">name of the PVS site</param>
        public static XenRef<PVS_site> introduce(Session session, string _name)
        {
            return XenRef<PVS_site>.Create(session.proxy.pvs_site_introduce(session.uuid, (_name != null) ? _name : "").parse());
        }

        /// <summary>
        /// Introduce new PVS site
        /// Experimental. First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_name">name of the PVS site</param>
        public static XenRef<Task> async_introduce(Session session, string _name)
        {
            return XenRef<Task>.Create(session.proxy.async_pvs_site_introduce(session.uuid, (_name != null) ? _name : "").parse());
        }

        /// <summary>
        /// Remove a site's meta data
        /// Experimental. First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_site">The opaque_ref of the given pvs_site</param>
        public static void forget(Session session, string _pvs_site)
        {
            session.proxy.pvs_site_forget(session.uuid, (_pvs_site != null) ? _pvs_site : "").parse();
        }

        /// <summary>
        /// Remove a site's meta data
        /// Experimental. First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_site">The opaque_ref of the given pvs_site</param>
        public static XenRef<Task> async_forget(Session session, string _pvs_site)
        {
            return XenRef<Task>.Create(session.proxy.async_pvs_site_forget(session.uuid, (_pvs_site != null) ? _pvs_site : "").parse());
        }

        /// <summary>
        /// Update the name of the PVS site
        /// Experimental. First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_site">The opaque_ref of the given pvs_site</param>
        /// <param name="_value">name to be used</param>
        public static void set_name(Session session, string _pvs_site, string _value)
        {
            session.proxy.pvs_site_set_name(session.uuid, (_pvs_site != null) ? _pvs_site : "", (_value != null) ? _value : "").parse();
        }

        /// <summary>
        /// Update the name of the PVS site
        /// Experimental. First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_site">The opaque_ref of the given pvs_site</param>
        /// <param name="_value">name to be used</param>
        public static XenRef<Task> async_set_name(Session session, string _pvs_site, string _value)
        {
            return XenRef<Task>.Create(session.proxy.async_pvs_site_set_name(session.uuid, (_pvs_site != null) ? _pvs_site : "", (_value != null) ? _value : "").parse());
        }

        /// <summary>
        /// Return a list of all the PVS_sites known to the system.
        /// Experimental. First published in .
        /// </summary>
        /// <param name="session">The session</param>
        public static List<XenRef<PVS_site>> get_all(Session session)
        {
            return XenRef<PVS_site>.Create(session.proxy.pvs_site_get_all(session.uuid).parse());
        }

        /// <summary>
        /// Get all the PVS_site Records at once, in a single XML RPC call
        /// </summary>
        /// <param name="session">The session</param>
        public static Dictionary<XenRef<PVS_site>, PVS_site> get_all_records(Session session)
        {
            return XenRef<PVS_site>.Create<Proxy_PVS_site>(session.proxy.pvs_site_get_all_records(session.uuid).parse());
        }

        /// <summary>
        /// Unique identifier/object reference
        /// Experimental. First published in .
        /// </summary>
        public virtual string uuid
        {
            get { return _uuid; }
            set
            {
                if (!Helper.AreEqual(value, _uuid))
                {
                    _uuid = value;
                    Changed = true;
                    NotifyPropertyChanged("uuid");
                }
            }
        }
        private string _uuid;

        /// <summary>
        /// Name of the PVS site. Must match name configured in PVS
        /// Experimental. First published in .
        /// </summary>
        public virtual string name
        {
            get { return _name; }
            set
            {
                if (!Helper.AreEqual(value, _name))
                {
                    _name = value;
                    Changed = true;
                    NotifyPropertyChanged("name");
                }
            }
        }
        private string _name;

        /// <summary>
        /// The SR used by PVS proxy for the cache
        /// Experimental. First published in .
        /// </summary>
        public virtual List<XenRef<PVS_cache_storage>> cache_storage
        {
            get { return _cache_storage; }
            set
            {
                if (!Helper.AreEqual(value, _cache_storage))
                {
                    _cache_storage = value;
                    Changed = true;
                    NotifyPropertyChanged("cache_storage");
                }
            }
        }
        private List<XenRef<PVS_cache_storage>> _cache_storage;

        /// <summary>
        /// The set of PVS servers in the site
        /// Experimental. First published in .
        /// </summary>
        public virtual List<XenRef<PVS_server>> servers
        {
            get { return _servers; }
            set
            {
                if (!Helper.AreEqual(value, _servers))
                {
                    _servers = value;
                    Changed = true;
                    NotifyPropertyChanged("servers");
                }
            }
        }
        private List<XenRef<PVS_server>> _servers;

        /// <summary>
        /// The set of proxies associated with the site
        /// Experimental. First published in .
        /// </summary>
        public virtual List<XenRef<PVS_proxy>> proxies
        {
            get { return _proxies; }
            set
            {
                if (!Helper.AreEqual(value, _proxies))
                {
                    _proxies = value;
                    Changed = true;
                    NotifyPropertyChanged("proxies");
                }
            }
        }
        private List<XenRef<PVS_proxy>> _proxies;
    }
}