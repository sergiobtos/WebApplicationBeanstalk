using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace WebApplicationBeanstalk.Models
{
    public class BucketsXMovie
    {
        public List<String> Buckets { get; set; }
        public Movie Movie { get; set; } = new Movie();
        public string SelectedBucket { get; set; }
        public IFormFile Cover { get; set; }
        public IFormFile Video { get; set; }
        public string Email { get; set; }

    }
}
