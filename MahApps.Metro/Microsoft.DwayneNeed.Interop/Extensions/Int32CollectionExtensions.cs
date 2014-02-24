using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Microsoft.DwayneNeed.Extensions
{
    public static class Int32CollectionExtensions
    {
        public static void Add(this Int32Collection _this, Tuple<int, int, int> tuple)
        {
            _this.Add(tuple.Item1);
            _this.Add(tuple.Item2);
            _this.Add(tuple.Item3);
        }
    }
}
