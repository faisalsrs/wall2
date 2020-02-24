using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace theWall.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Display(Name = "Name")]
        [Required]
        [MinLength(2)]
        [RegularExpression("^[A-Za-z ]+$", ErrorMessage = "Name can only have letters and spaces no numbers!")]
        public string fname { get; set; }
        [Display(Name = "Alias")]
        [RegularExpression("^[0-9A-Za-z]+$", ErrorMessage = "Alias can only have letters and numbers no spaces!")]
        [MinLength(2)]
        [Required]
        public string alias { get; set; }
        [Display(Name = "Email")]
        [EmailAddress]
        [Required]
        [MinLength(2)]
        public string email { get; set; }
        [MinLength(8, ErrorMessage = "Must be longers than 8 characters")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("password")]
        [NotMapped]
        public string cpassword { get; set; }
        public DateTime createdAt { get; set; } = DateTime.Now;
        public DateTime updatedAt { get; set; } = DateTime.Now;
        public List<Post> postsCreated { get; set; }
        public List<Vote> Votes { get; set; }

    }
}