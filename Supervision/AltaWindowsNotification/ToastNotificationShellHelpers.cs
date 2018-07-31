using System;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using MS.WindowsAPICodePack.Internal;

namespace AltaWindowsNotification
{
    internal enum STGM : long
    {
        STGM_READ = 0x00000000L,
        STGM_WRITE = 0x00000001L,
        STGM_READWRITE = 0x00000002L,
        STGM_SHARE_DENY_NONE = 0x00000040L,
        STGM_SHARE_DENY_READ = 0x00000030L,
        STGM_SHARE_DENY_WRITE = 0x00000020L,
        STGM_SHARE_EXCLUSIVE = 0x00000010L,
        STGM_PRIORITY = 0x00040000L,
        STGM_CREATE = 0x00001000L,
        STGM_CONVERT = 0x00020000L,
        STGM_FAILIFTHERE = 0x00000000L,
        STGM_DIRECT = 0x00000000L,
        STGM_TRANSACTED = 0x00010000L,
        STGM_NOSCRATCH = 0x00100000L,
        STGM_NOSNAPSHOT = 0x00200000L,
        STGM_SIMPLE = 0x08000000L,
        STGM_DIRECT_SWMR = 0x00400000L,
        STGM_DELETEONRELEASE = 0x04000000L,
    }

    internal static class ShellIIDGuid
    {
        internal const string IShellLinkW = "000214F9-0000-0000-C000-000000000046";
        internal const string CShellLink = "00021401-0000-0000-C000-000000000046";
        internal const string IPersistFile = "0000010b-0000-0000-C000-000000000046";
        internal const string IPropertyStore = "886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99";
    }

    [ComImport,
    Guid(ShellIIDGuid.IShellLinkW),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IShellLinkW
    {
        uint GetPath(
            [Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile,
            int cchMaxPath,
            //ref _WIN32_FIND_DATAW pfd,
            IntPtr pfd,
            uint fFlags);
        uint GetIDList(out IntPtr ppidl);
        uint SetIDList(IntPtr pidl);
        uint GetDescription(
            [Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile,
            int cchMaxName);
        uint SetDescription(
            [MarshalAs(UnmanagedType.LPWStr)] string pszName);
        uint GetWorkingDirectory(
            [Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir,
            int cchMaxPath
            );
        uint SetWorkingDirectory(
            [MarshalAs(UnmanagedType.LPWStr)] string pszDir);
        uint GetArguments(
            [Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs,
            int cchMaxPath);
        uint SetArguments(
            [MarshalAs(UnmanagedType.LPWStr)] string pszArgs);
        uint GetHotKey(out short wHotKey);
        uint SetHotKey(short wHotKey);
        uint GetShowCmd(out uint iShowCmd);
        uint SetShowCmd(uint iShowCmd);
        uint GetIconLocation(
            [Out(), MarshalAs(UnmanagedType.LPWStr)] out StringBuilder pszIconPath,
            int cchIconPath,
            out int iIcon);
        uint SetIconLocation(
            [MarshalAs(UnmanagedType.LPWStr)] string pszIconPath,
            int iIcon);
        uint SetRelativePath(
            [MarshalAs(UnmanagedType.LPWStr)] string pszPathRel,
            uint dwReserved);
        uint Resolve(IntPtr hwnd, uint fFlags);
        uint SetPath(
            [MarshalAs(UnmanagedType.LPWStr)] string pszFile);
    }

    [ComImport,
    Guid(ShellIIDGuid.IPersistFile),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IPersistFile
    {
        uint GetCurFile(
            [Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile
        );
        uint IsDirty();
        uint Load(
            [MarshalAs(UnmanagedType.LPWStr)] string pszFileName,
            [MarshalAs(UnmanagedType.U4)] STGM dwMode);
        uint Save(
            [MarshalAs(UnmanagedType.LPWStr)] string pszFileName,
            bool fRemember);
        uint SaveCompleted(
            [MarshalAs(UnmanagedType.LPWStr)] string pszFileName);
    }
    [ComImport]
    [Guid(ShellIIDGuid.IPropertyStore)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IPropertyStore
    {
        uint GetCount([Out] out uint propertyCount);
        uint GetAt([In] uint propertyIndex, out PropertyKey key);
        uint GetValue([In] ref PropertyKey key, [Out] PropVariant pv);
        uint SetValue([In] ref PropertyKey key, [In] PropVariant pv);
        uint Commit();
    }


    [ComImport,
    Guid(ShellIIDGuid.CShellLink),
    ClassInterface(ClassInterfaceType.None)]
    internal class CShellLink { }

    public static class ErrorHelper
    {
        public static void VerifySucceeded(uint hresult)
        {
            if (hresult > 1)
            {
                throw new Exception("Failed with HRESULT: " + hresult.ToString("X"));
            }
        }
    }
}
