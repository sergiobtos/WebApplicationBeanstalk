using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.S3;
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

        public HomeController(IAmazonDynamoDB dynamoDBClient,IAmazonS3 s3Client)
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
        public IActionResult LogIn (string email, string password)
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
            return View("Movies", new UserXMovies()
            {
                User = services.GetUser(email).Result,
                Movies = services.GetMovies().Result
            });
        }


        [HttpGet]
        public IActionResult MoviesDetails(string email, string movieId)
        {
            AWSServices services = new AWSServices(dynamoDBClient, s3Client);
            return View("MoviesDetails", new UserXMovie()
            {
                User = services.GetUser(email).Result,
                Movie = services.GetMovie(movieId,false).Result
            });
        }

        [HttpGet]
        public ActionResult DownloadMovie(string Id)
        {
            AWSServices services = new AWSServices(dynamoDBClient, s3Client);
            string Tmp = AppDomain.CurrentDomain.BaseDirectory;
            Movie movie = services.GetMovie(Id, true).Result;
            return PhysicalFile(Tmp + movie.Id + movie.Video.GetType(), "video/avi", movie.Title);
        }

        [HttpPost]
        public IActionResult AddComment(string email, string movieId, string comment,int rate)
        {
            AWSServices services = new AWSServices(dynamoDBClient, s3Client);
            services.AddComment(email, movieId, comment, rate);
            return MoviesDetails(email, movieId);
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
