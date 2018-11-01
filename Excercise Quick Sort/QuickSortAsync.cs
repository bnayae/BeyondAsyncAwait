using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bnaya.Samples
{
    public static class QuickSortAsync
    {
        private const int DEPTH_LIMIT = 4;

        private static readonly Random _rnd = new Random(Guid.NewGuid().GetHashCode());

        public static async Task<IEnumerable<T>> SortAsync<T>(IEnumerable<T> data, int depth = 0)
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

            // TODO: ValueTask
            Task<IEnumerable<T>> leftTask;
            Task<IEnumerable<T>> rightTask;
            depth++;
            if (depth > DEPTH_LIMIT)
            {
                leftTask = Task.FromResult(Sort(left));
                rightTask = Task.FromResult(Sort(right));
            }
            else
            {
                if (left.Count > 0)
                    leftTask = Task.Run(() => SortAsync(left, depth));
                else
                    leftTask = Task.FromResult<IEnumerable<T>>(left);
                if (right.Count > 0)
                    rightTask = Task.Run(() => SortAsync(right, depth));
                else
                    rightTask = Task.FromResult<IEnumerable<T>>(right);
            }

            IEnumerable<T>[] leftRight = await Task.WhenAll(leftTask, rightTask).ConfigureAwait(false);

            IEnumerable<T> result = Merger();
            return result;

            IEnumerable<T> Merger()
            {
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
