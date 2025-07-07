namespace turfbooking.Models
{
    public class SlotPrice
    {
        public int Id { get; set; }
        public  DateTime? Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public decimal Price { get; set; }
        public int CourtId { get; set; }
        public Court Court { get; set; }

    }
}
