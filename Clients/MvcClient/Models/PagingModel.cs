namespace BookOnline.MvcClient.Models
{
    public class PagingModel
    {
        public int PageSize { get; set; } = 20;
        public int PageIndex { get; set; } = 1;
        public int? TotalRows { get; set; }
        public int TotalPages { get; set; }

    }
}
