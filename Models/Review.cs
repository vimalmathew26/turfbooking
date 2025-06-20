﻿using System.ComponentModel.DataAnnotations;

namespace turfbooking.Models
{
    public class Review
    {
        public int Id { get; set; }

        public int GroundId { get; set; }
        [Required]
        public int BookingId { get; set; }
        [Range(1, 5)]
        public int Rating { get; set; }
        [MaxLength(500)]
        public string Comment { get; set; }
        public bool IsVisible { get; set; } = true;

        public Ground Ground { get; set; }
        public Booking Booking { get; set; }

        public Review()
        {
            BookingId = 0;
            Rating = 0;
            Comment = string.Empty;
            IsVisible = false;
            Ground = new Ground()
            {
                GroundName = "DefaultGround",
                PhotoPath = "default.jpg",
                Location = "Default Location",
                Description = "Default Description",
                PricePerHour = 50,
                SupportedSports = "Default Sport"
            };
        }
    }

}
