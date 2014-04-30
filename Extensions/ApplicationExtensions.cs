using System;
using System.IO;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Runtime.InteropServices;
using System.Runtime.ConstrainedExecution;
using System.Deployment.Application;
using Microsoft.Win32;

namespace Common.Wpf.Extensions
{
    public enum HostType
    {
        HostTypeDefault = 0x0,
        HostTypeAppLaunch = 0x1,
        HostTypeCorFlag = 0x2
    }

    // Taken from System.Windows.Forms.UnsafeNativeMethods
    [StructLayout(LayoutKind.Sequential), SuppressUnmanagedCodeSecurity]
    internal class ProcessInformation
    {
        public IntPtr hProcess = IntPtr.Zero;
        public IntPtr hThread = IntPtr.Zero;
        public int dwProcessId;
        public int dwThreadId;
        private static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
        ~ProcessInformation()
        {
            close();
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        internal void close()
        {
            if ((hProcess != IntPtr.Zero) && (hProcess != INVALID_HANDLE_VALUE))
            {
                CloseHandle(new HandleRef(this, hProcess));
                hProcess = INVALID_HANDLE_VALUE;
            }
            if ((hThread != IntPtr.Zero) && (hThread != INVALID_HANDLE_VALUE))
            {
                CloseHandle(new HandleRef(this, hThread));
                hThread = INVALID_HANDLE_VALUE;
            }
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        private static extern bool CloseHandle(HandleRef handle);
    }

    public static class ApplicationExtensions
    {
        [DllImport("clr.dll", CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = false)]
        internal static extern void CorLaunchApplication(uint hostType, string applicationFullName, int manifestPathsCount, string[] manifestPaths, int activationDataCount, string[] activationData, ProcessInformation processInformation);

        // Originally from System.Windows.Forms.Application, changed to suit needs
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode), SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static void Restart(this System.Windows.Application application)
        {
            if (Assembly.GetEntryAssembly() == null)
            {
                throw new NotSupportedException("RestartNotSupported");
            }
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                string updatedApplicationFullName = ApplicationDeployment.CurrentDeployment.UpdatedApplicationFullName;

                if (System.Windows.Application.Current != null)
                    System.Windows.Application.Current.Shutdown();

                CorLaunchApplication((uint) HostType.HostTypeDefault, updatedApplicationFullName, 0, null, 0, null, new ProcessInformation());
            }
        }

        public static void SetStartWithWindows(this System.Windows.Application application, bool value)
        {
            string applicationName = Assembly.GetEntryAssembly().GetName().Name;

            SetStartWithWindows(application, applicationName, value);
        }

        public static void SetStartWithWindows(this System.Windows.Application application, string applicationName, bool value)
        {
            string applicationPath = string.Format("\"{0}\"", Assembly.GetEntryAssembly().Location);

            SetStartWithWindows(application, applicationName, applicationPath, value);
        }

        public static void SetStartWithWindows(this System.Windows.Application application, string applicationName, string applicationPath, bool value)
        {
            // Open the regsitry key
            RegistryKey regkey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);

            // If we couldn't get the key then stop
            if (regkey == null)
                return;

            // Delete any existing key
            regkey.DeleteValue(applicationName, false);

            // If auto start should not be on then we're done
            if (!value)
                return;

            // Set the registry key
            regkey.SetValue(applicationName, applicationPath);
        }
    }
}
