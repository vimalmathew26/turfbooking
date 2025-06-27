using turfbooking.Models;
using turfbooking.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
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

            TimeSpan startTime = Ground.StartTime.TimeOfDay;
            TimeSpan endTime = Ground.EndTime.TimeOfDay;

            TimeSpan duration = Ground.SlotDuration;



            for (int dayOffset = 0; dayOffset < 7; dayOffset++)
            {
                DateTime CurrentDate = DateTime.Today.AddDays(dayOffset);


                for (TimeSpan time = startTime; time + duration <= endTime; time += duration)
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
                        await _context.SaveChangesAsync();

                    }

                }


            }
        }

        public async Task UpdateDefaultSlots(int groundId)
        {

            var existingSlots = await _context.Slots.Where(s => s.GroundId == groundId).ToListAsync();
            if (existingSlots.Any())
            {
                var bookingsToRemove = await _context.Bookings.Where(b => existingSlots.Select(s => s.Id).Contains(b.SlotId)).ToListAsync();
                _context.Bookings.RemoveRange(bookingsToRemove);
                _context.Slots.RemoveRange(existingSlots);
                await _context.SaveChangesAsync();
            }

            var Ground = await _context.Grounds.FindAsync(groundId);

            TimeSpan startTime = Ground.StartTime.TimeOfDay;
            TimeSpan endTime = Ground.EndTime.TimeOfDay;

            TimeSpan duration = Ground.SlotDuration;



            for (int dayOffset = 0; dayOffset < 7; dayOffset++)
            {
                DateTime CurrentDate = DateTime.Today.AddDays(dayOffset);


                for (TimeSpan time = startTime; time + duration <= endTime; time += duration)
                {
                    var slot = new Slot
                    {
                        GroundId = groundId,
                        StartTime = time,
                        EndTime = time + duration,
                        BookingDate = CurrentDate,
                        Status = Slot.SlotStatus.Available
                    };

                }
            }
        }
    }
}

