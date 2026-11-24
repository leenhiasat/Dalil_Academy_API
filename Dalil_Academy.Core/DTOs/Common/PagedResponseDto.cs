namespace Dalil_Academy.Core.DTOs.Common;

public record PagedResponseDto<T>(
    IEnumerable<T> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages
);