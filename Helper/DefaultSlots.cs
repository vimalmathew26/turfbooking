using turfbooking.Models;
using turfbooking.Data;
using Microsoft.EntityFrameworkCore;
namespace turfbooking.Helper
{
    public class DefaultSlots
    {

        public readonly AppDbContext _context;

        public DefaultSlots(AppDbContext context)
        {
            _context = context;
        }
        public async Task SetDefaultSlots(int groundId)
        {

            var Ground = await _context.Grounds.FindAsync(groundId);

            DateTime startTime = Ground.StartTime;
            DateTime endTime = Ground.EndTime;

            TimeSpan duration = Ground.duration;


            for (int dayOffset=0;dayOffset<7;dayOffset++)
            {
                DateTime CurrentDate = DateTime.Today.AddDays(dayOffset);

                
                for (TimeSpan time = startTime; time < endTime; time += duration)
                {
                    var slot = new Slot
                    {
                        GroundId = groundId,
                        StartTime = time,
                        EndTime = time + duration,
                        BookingDate = CurrentDate,
                        Status = Slot.SlotStatus.Available
                    };
                    
                    var existingSlot = await _context.Slots
                                      .FirstOrDefaultAsync(s => s.GroundId == groundId && s.BookingDate.Date == CurrentDate.Date && s.StartTime == time);
                    if (existingSlot == null)
                    {
                        _context.Slots.Add(slot);
                    }
                }

                
            }
        }
    }
}
