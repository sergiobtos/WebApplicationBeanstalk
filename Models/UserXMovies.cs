using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplicationBeanstalk.Models
{
    public class UserXMovies
    {
        public User User { get; set; }
        public List<Movie> Movies {get;set;}

    }
}
