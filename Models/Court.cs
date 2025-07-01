using System.ComponentModel.DataAnnotations;

namespace turfbooking.Models
{
    public class Court
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
        [Required]
        public TimeSpan Duration { get; set; }
        public int GroundId { get; set; }
        public Ground Ground { get; set; }
        

    }
}