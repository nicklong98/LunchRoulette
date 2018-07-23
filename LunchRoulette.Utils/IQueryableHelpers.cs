using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LunchRoulette.Utils.IQueryableHelpers
{

    public class QueryableExtender<T>
    {
        protected IQueryable<T> InvokeOn { get; }

        public QueryableExtender(IQueryable<T> invokeOn)
        {
            InvokeOn = invokeOn;
        }

        public async Task<T> SingleOrThrowAsync<E>() where E : Exception
        {
            if (InvokeOn == null) throw new ArgumentException(nameof(InvokeOn));
            var list = InvokeOn as IList<T>;
            if (list != null)
            {
                switch (list.Count)
                {
                    case 0:
                        throw Activator.CreateInstance<E>();
                    case 1:
                        return list[1];
                }
            }
            else
            {
                using (var enumerator = InvokeOn.ToAsyncEnumerable().GetEnumerator())
                {
                    if (!await enumerator.MoveNext())
                        throw Activator.CreateInstance<E>();
                    var current = enumerator.Current;
                    if (!await enumerator.MoveNext())
                        return current;
                }
            }
            throw Activator.CreateInstance<E>();
        }

        public async Task<T> SingleOrThrowAsync<E>(Func<T, bool> predicate) where E : Exception
        {
            return await InvokeOn.Where(predicate).AsQueryable().Extend().SingleOrThrowAsync<E>();
        }

        public T SingleOrThrow<E>() where E : Exception
        {
            if (InvokeOn == null) throw new ArgumentNullException(nameof(InvokeOn));
            IList<T> list = InvokeOn as IList<T>;
            if (list != null)
            {
                switch (list.Count)
                {
                    case 0:
                        throw Activator.CreateInstance<E>();
                    case 1:
                        return list[0];
                }
            }
            else
            {
                using (IEnumerator<T> enumerator = InvokeOn.GetEnumerator())
                {
                    if (!enumerator.MoveNext())
                        throw Activator.CreateInstance<E>();
                    T current = enumerator.Current;
                    if (!enumerator.MoveNext())
                        return current;
                }
            }
            throw Activator.CreateInstance<E>();
        }
    }

    public static class IQueryableExtensions
    {
        public static QueryableExtender<T> Extend<T>(this IQueryable<T> source)
        {
            return new QueryableExtender<T>(source);
        }

        public static QueryableExtender<T> Extend<T>(this IAsyncEnumerable<T> source)
        {
            return new QueryableExtender<T>(source.ToEnumerable().AsQueryable());
        }
    }
}