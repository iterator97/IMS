using System;
using System.Collections.Generic;

namespace IMS.Application.Wrappes
{
    public sealed record PagedResult<T>(
        IReadOnlyList<T> Items,
        int PageNumber,
        int PageSize,
        int TotalCount)
    {
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    }
}
