using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using TA.Service.Interface;
using Microsoft.Extensions.Configuration;
using System.Linq;
using TA.Domain;
using System.Collections.Generic;

namespace TweetFunctions
{
    public class Function1
    {
       // private readonly IRepository _repository;
        private readonly HttpClient _httpclient;
        private readonly IUserService userService;
        private readonly ITweetService tweetService;
        private readonly IConfiguration configuration;

        public Function1(HttpClient httpClient, IUserService userService, ITweetService tweetService, IConfiguration configuration)
        {
            //_repository = repository;
            _httpclient = httpClient;
            this.tweetService = tweetService;
            this.userService = userService;
        }

        [FunctionName("GetAllUsers")]
        public async Task<IActionResult> GetUserdetails(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "users/all")] HttpRequest req,
           ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            //List<UserModel> response = new List<UserModel>();
            var response = userService.GetAllUsers().ToList();
            if (response != null)
            {
                return new OkObjectResult(response);
            }
            else
            {
                return new OkObjectResult("User not found");
            }
            return new OkObjectResult("Error");
        }

        [FunctionName("GetAllTweets")]
        public async Task<IActionResult> GetTweetdetails(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "all")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            //List<TweetModel> response = new List<TweetModel>();
            var response = tweetService.GetAllTweets().ToList();
            if (response != null)
            {
                return new OkObjectResult(response);
            }
            else
            {
                return new OkObjectResult("Tweet not found");
            }
            return new OkObjectResult("Error");
        }
        [FunctionName("GetAllComments")]
        public async Task<IActionResult> GetComments(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get",  Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            //List<TweetCommentsModel> response = new List<TweetCommentsModel>();
            var response = tweetService.GetAllComments().ToList();
            if (response != null)
            {
                return new OkObjectResult(response);
            }
            else
            {
                return new OkObjectResult("No Comments");
            }
            return new OkObjectResult("Error");
        }

        [FunctionName("GettweetbyID")]
        public async Task<IActionResult> GettweetbyID(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "tweetById/{id}")] HttpRequest req,
            ILogger log, string id)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            //List<TweetModel> response = new List<TweetModel>();
            var response = tweetService.GetTweetById(id);
            if (response != null)
            {
                return new OkObjectResult(response);
            }
            else
            {
                return new OkObjectResult("No tweet");
            }
            return new OkObjectResult("Error");
        }

        //[FunctionName("MyHttpTrigger")]
        //public async Task<IActionResult> Run(
        //    [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
        //    ILogger log)
        //{
        //    var response = await _client.GetAsync("https://tweetapi.azurewebsites.net/home");
        //    //var message = _service.GetMessage();
        //    //dynamic data = JsonConvert.DeserializeObject(response.ToString());
        //    return new OkObjectResult(response.RequestMessage);
        //}

        [FunctionName("LoginUser")]
        public async Task<IActionResult> Loginapi(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Login")][FromBody] UserLoginModel user,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            //List<TweetModel> response = new List<TweetModel>();
            var response = userService.Login(user);
            if (response != null)
            {
                return new OkObjectResult("Login Sucessfull");
            }
            else
            {
                return new OkObjectResult("Login Failed");
            }
            return new OkObjectResult("Error");
        }

        [FunctionName("Register")]
        public async Task<IActionResult> Register(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Register")][FromBody] UserModel user,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            //List<TweetModel> response = new List<TweetModel>();
            var response = userService.Register(user);
            if (response == false)
            {
                return new OkObjectResult("User exists");
            }
            else if (response == true)
            {
                return new OkObjectResult("Register Sucessfull");
            }
            return new OkObjectResult("Error");
        }

        [FunctionName("ChangePassword")]
        public async Task<IActionResult> ChangePassword(string logid,
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "ChangePassword/{logid}")][FromBody] ChangePasswordModel user,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            //List<TweetModel> response = new List<TweetModel>();
            var response = userService.ChangePassword(logid, user);
            if (response == false)
            {
                return new OkObjectResult("Password not changed");
            }
            else if (response == true)
            {
                return new OkObjectResult("Change password Sucessfull");
            }
            return new OkObjectResult("Error");
        }

        [FunctionName("ReserPassword")]
        public async Task<IActionResult> ResetPassword(
           [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "ResetPassword")][FromBody] ResetPasswordModel user,
           ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            //List<TweetModel> response = new List<TweetModel>();
            var userModel = userService.CheckUserExists(user.email);
            if (userModel != null)
            {
                var response = userService.ResetPassword(user);
                if (response == false)
                {
                    return new OkObjectResult("Password not changed");
                }
                else if (response == true)
                {
                    return new OkObjectResult("Change password Sucessfull");
                }
            }

            return new OkObjectResult("User not exists");
        }

        [FunctionName("GetUserbyID")]
        public async Task<IActionResult> Getusebyid(string id,
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetUserById/{id}")] HttpRequest req,
           ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            //List<TweetModel> response = new List<TweetModel>();


            var response = userService.GetUserById(id);
            if (response != null)
            {
                return new OkObjectResult(response);
            }
            else
            {
                return new OkObjectResult("user not found");
            }


            return new OkObjectResult("Error");
        }

        [FunctionName("GetUserbyUsername")]
        public async Task<IActionResult> Getuserbyusername(string name,
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetUserByUsername/{name}")] HttpRequest req,
           ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            //List<TweetModel> response = new List<TweetModel>();


            var response = userService.GetUserByUsername(name);
            if (response != null)
            {
                return new OkObjectResult(response);
            }
            else
            {
                return new OkObjectResult("user not found");
            }


            return new OkObjectResult("Error");
        }
        [FunctionName("GetTweetbyUsername")]
        public async Task<IActionResult> GetTweetbyUsername(string name,
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{name}")] HttpRequest req,
           ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            //List<TweetModel> response = new List<TweetModel>();


            var response = tweetService.GetAllTweetsOfUser(name);
            if (response != null)
            {
                return new OkObjectResult(response);
            }
            else
            {
                return new OkObjectResult("Tweet not found");
            }


            return new OkObjectResult("Error");
        }


    }
}
