using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Async;

namespace Bnaya.Samples
{
    public static class QuickSortAsync
    {
        private const int DEPTH_LIMIT = 4;

        private static readonly Random _rnd = new Random(Guid.NewGuid().GetHashCode());

        public static async IAsyncEnumerable<T> SortAsync<T>(IAsyncEnumerable<T> data, int depth = 0)
            where T : IComparable
        {
            var left = new List<T>();
            var right = new List<T>();

            T pivot = await data.FirstOrDefaultAsync();
            IAsyncEnumerable<T> items = data.Skip(1);
            await foreach (var item in items)
            {
                if (pivot.CompareTo(item) < 0)
                    right.Add(item);
                else
                    left.Add(item);
            }

            IAsyncEnumerable<T> leftTask;
            IAsyncEnumerable<T> rightTask;
            depth++;
            if (depth > DEPTH_LIMIT)
            {
                foreach (var l in Sort(right))
                    yield return l;
                yield return pivot;
                foreach (var l in Sort(right))
                    yield return l;
                yield break; 
            }
                if (left.Count > 0)
                    leftTask = SortAsync(left, depth);
                else
                    leftTask = AsyncEnumerable.Empty<T>();
                if (right.Count > 0)
                    rightTask = SortAsync(right, depth);
                else
                    rightTask = AsyncEnumerable.Empty<T>();


            IEnumerable<T>[] leftRight = await Task.WhenAll(leftTask, rightTask).ConfigureAwait(false);

            foreach (var l_ in leftRight[0])
            {
                yield return l_;
            }
            yield return pivot;

            foreach (var r_ in leftRight[1])
            {
                yield return r_;
            }
        }

        public static IEnumerable<T> Sort<T>(IEnumerable<T> data)
            where T : IComparable
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

            IEnumerable<T> lft = left;
            IEnumerable<T> rgt = right;
            if (left.Count > 0)
                lft = Sort(left);
            if (right.Count > 0)
                rgt = Sort(right);

            foreach (var l_ in lft)
            {
                yield return l_;
            }
            yield return pivot;

            foreach (var r_ in rgt)
            {
                yield return r_;
            }
        }
    }
}
