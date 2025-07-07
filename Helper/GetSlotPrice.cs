using turfbooking.Data;
using turfbooking.Models;
using turfbooking.Pages.Admin;
namespace turfbooking.Helper
{
    public class GetSlotPrice
    {

        private readonly AppDbContext _context; 
        public GetSlotPrice(AppDbContext context)
        {
            _context = context;
        }
        public List<SlotPrice> slotPrice { get; set; }
        public Court court { get; set; }
        public decimal GetPrice(DateTime bookingDate, TimeSpan startTime, TimeSpan endTime, int courtId)
        {
           
            var slotPrice = _context.SlotPrices
                .Where(sp => sp.CourtId == courtId && sp.Date == bookingDate.Date)
                .ToList();

            
            if (!slotPrice.Any())
            {
                slotPrice = _context.SlotPrices
                    .Where(sp => sp.CourtId == courtId && sp.Date == null)
                    .ToList();
            }

            
            foreach (var sp in slotPrice)
            {
                if (startTime >= sp.StartTime && endTime <= sp.EndTime)
                {
                    return sp.Price;
                }
            }

            
            var court = _context.Courts.FirstOrDefault(c => c.Id == courtId);
            return court.PricePerHour; 
        }


    }
}
