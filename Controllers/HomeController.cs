using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using WebApplicationBeanstalk.Models;
using WebApplicationBeanstalk.Service;

namespace WebApplicationBeanstalk.Controllers
{
    public class HomeController : Controller
    {
        private IAmazonDynamoDB dynamoDBClient;

        private IAmazonS3 s3Client;
        

        public HomeController(IAmazonDynamoDB dynamoDBClient, IAmazonS3 s3Client)
        {
            this.dynamoDBClient = dynamoDBClient;
            this.s3Client = s3Client;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RegistrationForm()
        {
            return View();
        }

        [HttpPost]
        public IActionResult LogIn(string email, string password)
        {

            AWSServices services = new AWSServices(dynamoDBClient, s3Client);
            User user = services.LogIn(email, password).Result;

            if (user == null)
                return View("Index");
            else
            {

                return Movies(email);
            }
        }

        [HttpGet]
        public IActionResult Movies(string email)
        { 
            AWSServices services = new AWSServices(dynamoDBClient, s3Client);
            UserXMovies userXMovies = new UserXMovies()
            {
                User = services.GetUser(email).Result,
                Movies = services.GetMovies().Result
            };
            return View("Movies", userXMovies );
        }

        [HttpPost]
        public IActionResult Movies()
        {
            
            return View();
        }

        [HttpPost]
        public IActionResult AddMovie(BucketsXMovie bucketsXMovie )
        {
            string Tmp = AppDomain.CurrentDomain.BaseDirectory;

            if (ModelState.IsValid)
            {

                var video = new MemoryStream();
                bucketsXMovie.Video.CopyTo(video);
                var cover = new MemoryStream();
                bucketsXMovie.Cover.CopyTo(cover);

                AWSServices services = new AWSServices(dynamoDBClient, s3Client);
                var result = services.UploadMovie(bucketsXMovie.SelectedBucket,
                    bucketsXMovie.Movie.Title,
                    video, 
                    cover
                  ).Result; 
            }
            return Movies(bucketsXMovie.Email);
         
        }

        [HttpGet]
        public IActionResult AddMovie(string email)
        {
            AWSServices services = new AWSServices(dynamoDBClient, s3Client);
            return View(new BucketsXMovie() { Buckets = services.GetBuckets(),Email=email });
        }


        [HttpGet]
        public IActionResult MovieDetails(string email, string movieId)
        {
            AWSServices services = new AWSServices(dynamoDBClient, s3Client);
            return View(new UserXMovie()
            {
                User = services.GetUser(email).Result,
                Movie = services.GetMovie(movieId).Result
            });
        }

        [HttpGet]
        public ActionResult DownloadMovie(string Id)
        {
            AWSServices services = new AWSServices(dynamoDBClient, s3Client);
            string Tmp = AppDomain.CurrentDomain.BaseDirectory;
            Movie movie = services.GetMovie(Id).Result;
            return PhysicalFile(Tmp + movie.Id + movie.Video.GetType(), "video/avi", movie.Title);
        }

        [HttpPost]
        public IActionResult AddComment(string email, string movieId, string comment,int rate)
        {
            AWSServices services = new AWSServices(dynamoDBClient, s3Client);
            services.AddComment(email, movieId, comment, rate);
            return MovieDetails(email, movieId);
        }

  
        [HttpPost]
        public IActionResult AddUser (User user)
        {
            AWSServices services = new AWSServices(dynamoDBClient, s3Client);
            if (ModelState.IsValid)
            {
                User newUser =  services.Register(user).Result;
                if(newUser != null)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return View("index");
                }
                
            }
            else
            {
                return View(user);
            }
            
        }



    }
}
