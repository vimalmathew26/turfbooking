using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace turfbooking.Models
{
    public class Ground
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ground name is required.")]
        [StringLength(100, ErrorMessage = "Ground name cannot exceed 100 characters.")]
        public required string GroundName { get; set; }

        //[BindProperty]
        //[Required(ErrorMessage = "Please upload a ground photo")]
        //[Display(Name = "Upload Photo")]
        ////public required IFormFile? Photo { get; set; }

        //[BindProperty]
        //[NotMapped]
        //[Required(ErrorMessage = "Photo is required")]
        //[Display(Name = "Upload Photo")]
        //public IFormFile? Photo { get; set; }

        public required string? PhotoPath { get; set; }

        [Required(ErrorMessage = "Location is required.")]
        [StringLength(100, ErrorMessage = "Location cannot exceed 100 characters.")]
        public required string Location { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        [Display(Name = "Ground Description")]
        public required string Description { get; set; }
        // E.g., "Artificial turf, 7-a-side, Lighting available, Parking available"

        [Required(ErrorMessage = "Price per hour is required.")]
        [Range(50, 10000, ErrorMessage = "Price per hour must be between ₹50 and ₹10,000.")]
        [Display(Name = "Price Per Hour (₹)")]
        public required decimal PricePerHour { get; set; }

        [Required(ErrorMessage = "Please specify supported sports.")]
        [StringLength(200, ErrorMessage = "Supported sports cannot exceed 200 characters.")]
        [Display(Name = "Supported Sports")]
        public required string SupportedSports { get; set; }
        // E.g., "Football, Cricket, Badminton"

        public bool IsActive { get; set; } = true;

        // Navigation properties for future use
        public ICollection<Slot> Slots { get; set; } = new List<Slot>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
