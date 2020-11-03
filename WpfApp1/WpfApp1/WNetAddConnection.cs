using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Configuration;

namespace WpfApp1
{
    enum ResourceScope
    {
        RESOURCE_CONNECTED = 1,
        RESOURCE_GLOBALNET,
        RESOURCE_REMEMBERED,
        RESOURCE_RECENT,
        RESOURCE_CONTEXT
    };

    enum ResourceType
    {
        RESOURCETYPE_ANY,
        RESOURCETYPE_DISK,
        RESOURCETYPE_PRINT,
        RESOURCETYPE_RESERVED
    };

    enum ResourceUsage
    {
        RESOURCEUSAGE_CONNECTABLE = 0x00000001,
        RESOURCEUSAGE_CONTAINER = 0x00000002,
        RESOURCEUSAGE_NOLOCALDEVICE = 0x00000004,
        RESOURCEUSAGE_SIBLING = 0x00000008,
        RESOURCEUSAGE_ATTACHED = 0x00000010,
        RESOURCEUSAGE_ALL = (RESOURCEUSAGE_CONNECTABLE | RESOURCEUSAGE_CONTAINER | RESOURCEUSAGE_ATTACHED),
    };

    enum ResourceDisplayType
    {
        RESOURCEDISPLAYTYPE_GENERIC,
        RESOURCEDISPLAYTYPE_DOMAIN,
        RESOURCEDISPLAYTYPE_SERVER,
        RESOURCEDISPLAYTYPE_SHARE,
        RESOURCEDISPLAYTYPE_FILE,
        RESOURCEDISPLAYTYPE_GROUP,
        RESOURCEDISPLAYTYPE_NETWORK,
        RESOURCEDISPLAYTYPE_ROOT,
        RESOURCEDISPLAYTYPE_SHAREADMIN,
        RESOURCEDISPLAYTYPE_DIRECTORY,
        RESOURCEDISPLAYTYPE_TREE,
        RESOURCEDISPLAYTYPE_NDSCONTAINER
    };

    [Flags]
    enum Flags_WNetAddConnection2
    {
        CONNECT_UPDATE_PROFILE = 1,
        CONNECT_UPDATE_RECENT = 2,
        CONNECT_TEMPORARY = 4,
        CONNECT_INTERACTIVE = 8,
        CONNECT_PROMPT = 16,
        CONNECT_NEED_DRIVE = 32,
        CONNECT_REFCOUNT = 64,
        CONNECT_REDIRECT = 128,
        CONNECT_LOCALDRIVE = 256,
        CONNECT_CURRENT_MEDIA = 512
    }

    [StructLayout(LayoutKind.Sequential)]
    class NETRESOURCE
    {
        public ResourceScope dwScope = 0;
        public ResourceType dwType = 0;
        public ResourceDisplayType dwDisplayType = 0;
        public ResourceUsage dwUsage = 0;
        public string lpLocalName = null;
        public string lpRemoteName = null;
        public string lpComment = null;
        public string lpProvider = null;
    };

    public static class NetworkHelper
    {
        [DllImport("mpr.dll", EntryPoint = "WNetAddConnection2A", CharSet = System.Runtime.InteropServices.CharSet.Ansi, SetLastError = true)]
        private static extern int WNetAddConnection2([In] NETRESOURCE netResource,
        string password, string username, Flags_WNetAddConnection2 flags);

        [DllImport("mpr.dll", EntryPoint = "WNetCancelConnection2A", CharSet = System.Runtime.InteropServices.CharSet.Ansi, SetLastError = true)]
        private static extern int WNetCancelConnection(string name, int flags, int force);

        public static int Connect(string remoteName, string userName, string password, bool alwaysPrompt, out string error)
        {
            error = string.Empty;
            NETRESOURCE res = new NETRESOURCE();
            res.dwType = ResourceType.RESOURCETYPE_ANY;
            res.lpLocalName = string.Empty;
            res.lpRemoteName = remoteName;

            Flags_WNetAddConnection2 flags = Flags_WNetAddConnection2.CONNECT_TEMPORARY | Flags_WNetAddConnection2.CONNECT_INTERACTIVE;
            if (alwaysPrompt)
            {
                flags |= Flags_WNetAddConnection2.CONNECT_PROMPT;
            }

            int stat = WNetAddConnection2(res, password, userName, flags);
            if (stat > 0)
            {
                try
                {
                    if (stat == 1219) stat = 1326;
                    throw new Win32Exception(stat);
                }
                catch (Win32Exception ex)
                {
                    error = ex.Message;
                }
            }
            return stat;
        }

        public static int Disconnect(string RemoteName, bool force, out string error)
        {
            error = string.Empty;
            int ret = WNetCancelConnection(RemoteName, 0, System.Convert.ToInt32(force));
            if (ret > 0)
            {
                try
                {
                    throw new Win32Exception(ret);
                }
                catch (Win32Exception ex)
                {
                    error = ex.Message;
                }
            }
            return ret;
        }

    }
}
