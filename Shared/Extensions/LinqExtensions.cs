using System.Collections.Generic;
using System.Linq;

namespace Shared.Extensions
{
    public static class LinqExtensions
    {
        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> enumerable, int chunkSize)
        {
            return enumerable
                .Select((e, i) => new { e, Index = i / chunkSize })
                .GroupBy(g => g.Index, g => g.e)
                .Select(g => g.Select(x => x));
        }
    }
}