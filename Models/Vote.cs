using System;
using System.ComponentModel.DataAnnotations;

namespace theWall.Models
{
    public class Vote
    {
        [Key]
        public int VoteId { get; set; } // primery key
        public int UserId { get; set; } //foreign key
        public int PostId { get; set; } //foreign key
        public bool IsUpvote { get; set; }
        public User UserVoted { get; set; } //navigation property
        public Post PostVoted { get; set; } //navigation property
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

    }
}