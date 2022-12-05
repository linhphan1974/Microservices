namespace BookOnline.MvcClient.Models.Dto
{
    public class BorrowItemDto
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string Title { get; set; }
        public string PictureUrl { get; set; }

    }
}
