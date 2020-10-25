using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplicationBeanstalk.Models
{
    public class MovieXRating
    {
        public Movie Movie { get; set; }
        public Rating Rating { get; set; }
        public User User { get; set; }

    }
}
