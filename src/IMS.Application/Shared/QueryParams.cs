using System;

namespace IMS.Application.Helpers
{
    public sealed class QueryParams
    {
        public const int MaxPageSize = 100;
        public int PageNumber { get; set; } = 1;

        private int _pageSize = 10;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = Math.Clamp(value, 1, MaxPageSize);
        }
        public string SortBy { get; set; } = "name";
        public string SortDirection { get; set; } = "asc";
    }
}
