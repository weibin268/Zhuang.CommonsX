using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zhuang.Commons.Utils
{
    public static class RandomUtil
    {
        public static T Choice<T>(T[] items)
        {
            var randomIndex = new Random(Guid.NewGuid().GetHashCode()).Next(0, items.Length - 1);
            return items[randomIndex];
        }

        public static T Choice<T>(IList<T> items)
        {
            return Choice(items.ToArray());
        }
    }
}
