using System;
using System.Collections.Generic;
using System.Text;

namespace Dalil_Academy.Core.DTOs
{
    public record PageResult<T>(List<T> Items, int page, int pageSize, int TotalCount)
    {
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / pageSize);
    }
}
