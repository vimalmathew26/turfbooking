using turfbooking.Models;
using turfbooking.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
namespace turfbooking.Helper
{
     public class DefaultSlots
    {

        public readonly AppDbContext _context;
        public readonly GetSlotPrice _getSlotPrice;

        public DefaultSlots(AppDbContext context, GetSlotPrice getSlotPrice)
        {
            _context = context;
            _getSlotPrice = getSlotPrice;
        }


        public async Task SetDefaultSlots(int groundId, int courtId)
        {
            var ground = await _context.Grounds.FindAsync(groundId);
            var court = await _context.Courts.FindAsync(courtId);

            TimeSpan startTime = court.StartTime.TimeOfDay;
            TimeSpan endTime = court.EndTime.TimeOfDay;
            TimeSpan duration = court.Duration;

            for (int dayOffset = 0; dayOffset < 7; dayOffset++)
            {
                DateTime currentDate = DateTime.Today.AddDays(dayOffset);

                for (TimeSpan time = startTime; time + duration <= endTime; time += duration)
                {
                    var existingSlot = await _context.Slots.FirstOrDefaultAsync(s =>
                        s.GroundId == groundId &&
                        s.CourtId == courtId &&
                        s.BookingDate.Date == currentDate.Date &&
                        s.StartTime == time &&
                        s.EndTime == time + duration);

                    if (existingSlot == null)
                    {
                        var newSlot = new Slot
                        {
                            GroundId = groundId,
                            StartTime = time,
                            EndTime = time + duration,
                            BookingDate = currentDate,
                            Status = Slot.SlotStatus.Available,
                            CourtId = court.Id,
                            price = _getSlotPrice.GetPrice(currentDate, time, time + duration, court.Id)
                        };

                        _context.Slots.Add(newSlot);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        existingSlot.price = _getSlotPrice.GetPrice(currentDate, time, time + duration, court.Id);
                        _context.Slots.Update(existingSlot);
                        await _context.SaveChangesAsync();
                    }
                }
            }
        }


        public async Task UpdateDefaultSlots(int groundId, int courtId)
        {

            var existingSlots = await _context.Slots.Where(s => s.GroundId == groundId && s.CourtId == courtId).ToListAsync();
            if (existingSlots.Any())
            {
                var bookingsToRemove = await _context.Bookings.Where(b => existingSlots.Select(s => s.Id).Contains(b.SlotId.Value)).ToListAsync();
                _context.Bookings.RemoveRange(bookingsToRemove);
                _context.Slots.RemoveRange(existingSlots);
                await _context.SaveChangesAsync();
            }

            var Ground = await _context.Grounds.FindAsync(groundId);
            var Court = await _context.Courts.FindAsync(courtId);


            TimeSpan startTime = Court.StartTime.TimeOfDay;
            TimeSpan endTime = Court.EndTime.TimeOfDay;

            TimeSpan duration = Court.Duration;



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
                        Status = Slot.SlotStatus.Available,
                        CourtId = Court.Id,
                        price = _getSlotPrice.GetPrice(CurrentDate, time, time + duration, Court.Id)
                    };

                    var existingSlot = await _context.Slots
                                      .FirstOrDefaultAsync(s => s.GroundId == groundId && s.CourtId == courtId && s.BookingDate.Date == CurrentDate.Date && s.StartTime == time);
                    if (existingSlot == null)
                    {
                        _context.Slots.Add(slot);
                      

                    }

                }


            }
        }
    }

}


    