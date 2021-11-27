using System;

namespace Shared
{
    public abstract class QueryStringParameters
    {
    }

    public sealed class PageQueryStringParameters
    {
        public const int MaxPageSize = 200;
        public const int DefaultPageSize = 50;

        private int _size = DefaultPageSize;

        public int Size
        {
            get => _size;
            set => _size = Math.Min(MaxPageSize, value);
        }

        public int Number { get; set; } = 1;
    }
}