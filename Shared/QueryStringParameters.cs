using System;

namespace Shared
{
    public abstract class QueryStringParameters
    {
    }

    public sealed class PaginationParameters
    {
        public const int MaxPageSize = 2000;
        public const int DefaultPageSize = 500;

        private readonly int _pageSize = DefaultPageSize;

        public int PageSize
        {
            get => _pageSize;
            init => _pageSize = Math.Min(MaxPageSize, value);
        }

        public int PageNumber { get; set; } = 1;
    }
}