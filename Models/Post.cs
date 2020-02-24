using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace theWall.Models
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }

        [Required(ErrorMessage = "Field is required")]
        [DataType(DataType.Text)]
        [MinLength(5, ErrorMessage = "No less than 5 characters")]
        [Display(Name = "Enter your post here:")]
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public int UserId { get; set; } //foreign key
        public User Creator { get; set; } // navigation propery
        public List<Vote> Votes { get; set; }

    }
}