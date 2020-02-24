using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace theWall.Models
{
    [NotMapped]
    public class successModel
    {
        public List<Post> allP { get; set; }
        public User userLogged { get; set; }
        public Post post { get; set; }

        public List<Vote> allv { get; set; }

    }
}