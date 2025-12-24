using ALAP.Entity.Models.Wapper;
using Microsoft.EntityFrameworkCore;

namespace ALAP.DAL
{
    public static class QueryableExtensions
    {
        public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
    this IQueryable<T> query, PagingModel paging)
        {
            var totalRecords = await query.CountAsync();
            var pageNumber = paging.PageNumber;
            var pageSize = paging.PageSize;

            var data = await query.Skip((paging.PageNumber - 1) * paging.PageSize)
                                  .Take(paging.PageSize)
                                  .ToListAsync();

            return new PagedResult<T>(data, totalRecords, pageNumber, pageSize);
        }
    }
}
