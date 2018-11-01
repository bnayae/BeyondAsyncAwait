using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bnaya.Samples
{
    public static class QuickSortSimplify
    {
        private static readonly Random _rnd = new Random(Guid.NewGuid().GetHashCode());

        public static IEnumerable<T> Sort<T>(IEnumerable<T> data)
            where T: IComparable
        {
            var left = new List<T>();
            var right = new List<T>();

            T pivot = data.FirstOrDefault();
            foreach (var item in data.Skip(1))
            {
                if (pivot.CompareTo(item) < 0)
                    right.Add(item);
                else
                    left.Add(item);
            }

            IEnumerable<T> l = left;
            IEnumerable<T> r = right;
            if (left.Count > 0)
                l = Sort(left);
            if (right.Count > 0)
                r = Sort(right);

            foreach (var l_ in l)
            {
                yield return l_;
            }
            yield return pivot;
            foreach (var r_ in r)
            {
                yield return r_;
            }
        }
    }
}
