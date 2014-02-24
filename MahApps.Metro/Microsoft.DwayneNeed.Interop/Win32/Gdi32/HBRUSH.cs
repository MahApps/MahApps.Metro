using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.DwayneNeed.Win32.Gdi32
{
    public class HBRUSH : HGDIOBJ
    {
        private HBRUSH()
        {
        }

        public HBRUSH(IntPtr handle)
        {
            SetHandle(handle);
        }

        public HBRUSH(COLORREF color)
        {
            HBRUSH hbrush = CreateSolidBrush(color.Value);
            this.SetHandle(hbrush.DangerousGetHandle());
            this.DangerousOwnsHandle = true;
        }

        #region PInvoke
        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        private static extern HBRUSH CreateSolidBrush(uint color);
        #endregion
    }
}
