using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace turfbooking.Models
{
    public class SlotPrice
    {
        public int Id { get; set; }
        public  DateTime? Date { get; set; }
        [Required]
        public TimeSpan StartTime { get; set; }
        [Required]
        public TimeSpan EndTime { get; set; }
        [Required]
        public decimal Price { get; set; }
        public int CourtId { get; set; }
        public Court? Court { get; set; }

    }
}
