namespace ALAP.Entity.Models.Wapper
{
    public class PagedResult<T>
    {
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public List<T> ListObjects { get; set; }

        public PagedResult(List<T> data, int totalRecords, int pageNumber, int pageSize)
        {
            ListObjects = data;
            TotalRecords = totalRecords;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}
