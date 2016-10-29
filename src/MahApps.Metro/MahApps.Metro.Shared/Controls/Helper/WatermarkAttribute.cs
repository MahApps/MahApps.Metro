using System;

namespace MahApps.Metro.Controls.Helper
{
#if NET4_5
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class WatermarkAttribute : Attribute
    {
        public WatermarkAttribute(string caption)
        {
            this.Caption = caption;
        }

        public string Caption { get; set; }
    }
#endif
}