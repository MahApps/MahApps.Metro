using System;

namespace MahApps.Metro.Controls.Helper
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class WatermarkAttribute : Attribute
    {
        public WatermarkAttribute(string caption)
        {
            this.Caption = caption;
        }

        public string Caption { get; set; }
    }
}