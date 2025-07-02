using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace turfbooking.Models
{
    public class Review
    {
        public int Id { get; set; }
       
        public int UserId { get; set; }
      
       
    
        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        [MaxLength(500)]
        public string Comment { get; set; }

        public bool IsVisible { get; set; } = true;
        public int GroundId { get; set; }

        [ValidateNever]
        public Ground? Ground { get; set; }
    }

}
