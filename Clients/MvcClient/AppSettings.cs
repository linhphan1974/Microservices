namespace BookOnline.MvcClient
{
    public class AppSettings
    {
        public string BasketUrl { get; set; }
        public string BorrowUrl { get; set; } 
        public string BookUrl { get; set; }
        public string AggregatorUrl { get; set; }
        public string SignalrHubUrl { get; set; }
        public int DefaultPageSize { get; set; }
    }
}
