using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Falling
{
    public static class ExtensionMethods
    {
        public static void OrderByInPlace<T, TResult>(this List<T> list, Func<T, TResult> comparison)
        {
            var sortedList = list.OrderBy(comparison);
            int i = 0;
            foreach (var element in sortedList)
            {
                list[i] = element;
                i++;
            }
        }
    }
}
