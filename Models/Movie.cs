using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;

namespace WebApplicationBeanstalk.Models
{
    [DynamoDBTable("Movie")]
    public class Movie
    {
        [DynamoDBHashKey]
        public string Id { get; set; }

        public string Title { get; set; }

        public S3Link Cover { get; set; }

        public S3Link Video { get; set; }

        [DynamoDBProperty(AttributeName = "Ratings")]
        public List<Rating> Ratings { get; set; }

    }
}
