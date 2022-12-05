namespace BookOnline.Borrowing.Api.Infrastucture.Queries
{
    public class BorrowDto
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public DateTime BorrowDate { get; set; }
        public int BorrowStatus { get; set; }
        public DateTime PickupDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public int ShipType { get; set; }
        public string Description { get; set; }
        public List<BorrowItemDto> Items { get; set; }
        public BorrowDto()
        {
            Items = new List<BorrowItemDto>();
        }
    }
}
