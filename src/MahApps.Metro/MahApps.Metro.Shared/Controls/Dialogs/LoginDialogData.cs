using System;
using System.Security;

namespace MahApps.Metro.Controls.Dialogs
{
    public class LoginDialogData
    {
        public string Username { get; internal set; }

        public string Password
        {
            [SecurityCritical]
            get
            {
                IntPtr ptr = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(this.SecurePassword);
                try
                {
                    return System.Runtime.InteropServices.Marshal.PtrToStringBSTR(ptr);
                }
                finally
                {
                    System.Runtime.InteropServices.Marshal.ZeroFreeBSTR(ptr);
                }
            }
        }

        public SecureString SecurePassword { get; internal set; }

        public bool ShouldRemember { get; internal set; }
    }
}