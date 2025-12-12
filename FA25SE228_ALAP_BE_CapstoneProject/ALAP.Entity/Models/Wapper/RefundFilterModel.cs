namespace ALAP.Entity.Models.Wapper
{
    public class RefundFilterModel : PagingModel
    {
        public long? EventId { get; set; }
        public bool? IsRefunded { get; set; }
    }
}
