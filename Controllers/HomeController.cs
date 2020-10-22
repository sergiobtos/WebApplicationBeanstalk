using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using WebApplicationBeanstalk.Models;
using WebApplicationBeanstalk.Service;

namespace WebApplicationBeanstalk.Controllers
{
    public class HomeController : Controller
    {
        AWSServices services = new AWSServices();
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
            User user = services.LogIn(email, password).Result;

            if (user == null)
                return View("Index");
            else
            {
                return View("Movies", new UserXMovies()
                {
                    User = user,
                    Movies = services.GetMovies().Result
                });
            }
        }

        [HttpPost]
        public IActionResult AddUser (User user)
        {
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
