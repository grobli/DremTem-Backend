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

        private int _size = DefaultPageSize;

        public int Size
        {
            get => _size;
            set => _size = Math.Min(MaxPageSize, value);
        }

        public int Number { get; set; } = 1;
    }
}