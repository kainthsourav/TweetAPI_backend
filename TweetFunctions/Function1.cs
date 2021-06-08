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
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Collections.Generic;
using App.Tweet.Service.Interface;
using App.Tweet.DAL;

namespace TweetFunctions
{
    public class Function1
    {
        private readonly IUserService _userService;
        private readonly ITweetService _tweetService;

        public Function1(IUserService userService, ITweetService tweetService)
        {
            _userService = userService;
            _tweetService = tweetService;
        }

        #region User Functions

        [FunctionName("Register")]
        public IActionResult Register(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Register")][FromBody] UserModel user,
            ILogger log)
        {
            log.LogInformation("Register function triggered.");

            var response = _userService.Register(user);
            if (response == false)
            {
                log.LogInformation("User already exists");
                return new OkObjectResult("User already exists");

            }
            else if (response == true)
            {
                return new OkObjectResult("Register Sucessfull");
            }
            return new OkObjectResult("Error");
        }


        [FunctionName("Login")]
        public IActionResult Loginapi(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Login")][FromBody] LoginModel user,
            ILogger log)
        {
            log.LogInformation("Login function triggered.");

            var response = _userService.Login(user);
            if (response != null)
            {
                return new OkObjectResult(response);
            }
            else
            {
                return new OkObjectResult("Login Failed");
            }
           
        }

        [FunctionName("ChangePassword")]
        public IActionResult ChangePassword(string logid,
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "ChangePassword/{logid}")][FromBody] ChangePasswordModel user,
            ILogger log)
        {
            log.LogInformation("ChangePassword function triggered.");
            var response = _userService.ChangePassword(logid, user);
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
        public IActionResult ResetPassword(
           [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "ResetPassword")][FromBody] ResetPasswordModel user,
           ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var userModel = _userService.CheckUserExists(user.email);
            if (userModel != null)
            {
                var response = _userService.ResetPassword(user);
                if (response == false)
                {
                    return new OkObjectResult("Password not changed");
                }
                else if (response == true)
                {
                    return new OkObjectResult("Change password Sucessfull");
                }
            }
            return new OkObjectResult("Bad Request");

        }

        [FunctionName("GetUserbyID")]
        public IActionResult Getusebyid(string id,
          [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetUserById/{id}")] HttpRequest req,
          ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var response = _userService.GetUserById(id);
            if (response != null)
            {
                return new OkObjectResult(response);
            }
            else
            {
                return new OkObjectResult("user not found");
            }

        }

        [FunctionName("GetUserbyUsername")]
        public IActionResult Getuserbyusername(string username,
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetUserByUsername/{username}")] HttpRequest req,
           ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var response = _userService.GetUserByUsername(username);
            if (response != null)
            {
                return new OkObjectResult(response);
            }
            else
            {
                return new OkObjectResult("user not found");
            }

        }

        [FunctionName("GetAllUsers")]
        public IActionResult GetUserdetails(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "users/all")] HttpRequest req,
           ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var response = _userService.GetAllUsers().ToList();
            if (response != null)
            {
                return new OkObjectResult(response);
            }
            else
            {
                return new OkObjectResult("User not found");
            }
        }
        #endregion

        [FunctionName("GetTweetsByUserName")]
        public IActionResult TweetsByUserName(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "{username}")] string username,ILogger log)
        {
            log.LogInformation("TweetsByUserName function triggered.");

            var tweetsByUserId = _tweetService.GetAllTweetsOfUser(username);
            if (tweetsByUserId != null)
            {
                for (int i = 0; i < tweetsByUserId.Count; i++)
                {
                    var tweetCommentsModels = _tweetService.GetTweetCommentsById(tweetsByUserId[i].tweetId);
                    if (tweetCommentsModels.Count != 0)
                    {
                        if (tweetCommentsModels[0].tweetId == tweetsByUserId[i].tweetId)
                        {
                            tweetsByUserId[i].commentsCount = tweetCommentsModels.Count;
                        }
                    }
                    tweetCommentsModels = null;
                }


                var userTweets = (from t1 in tweetsByUserId
                                  orderby t1.createdAt descending
                                  select new { t1.tweetId, t1.userId, t1.tweetTag, t1.tweetDescription, t1.createdAt, t1.commentsCount, t1.likesCount }).ToList();

                return new OkObjectResult(userTweets);

            }
            return new OkObjectResult("No Tweets Found");
        }


        [FunctionName("SearchLikeUserTweet")]
        public IActionResult SearchLikeUserTweet(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "user/searchbbox/{username}")] string username,ILogger log)
        {
            log.LogInformation("TweetsByUserName function triggered.");

            var tweetsByUserId = _tweetService.GetAllTweetSearchUser(username);

            var users = _userService.GetAllUsers();

            if (tweetsByUserId != null)
            {
                for (int i = 0; i < tweetsByUserId.Count; i++)
                {

                    List<TweetCommentsModel> tweetCommentsModels = new List<TweetCommentsModel>();
                    tweetCommentsModels = _tweetService.GetTweetCommentsById(tweetsByUserId[i].tweetId);
                    if (tweetCommentsModels.Count != 0)
                    {
                        if (tweetCommentsModels[0].tweetId == tweetsByUserId[i].tweetId)
                        {
                            tweetsByUserId[i].commentsCount = tweetCommentsModels.Count;
                        }
                    }
                    tweetCommentsModels = null;
                }


                var usr = (from t1 in tweetsByUserId
                           orderby t1.createdAt descending
                           join t2 in users
                           on t1.userId equals t2.userId
                           select new { t1.tweetId, t1.userId, t1.tweetTag, t1.tweetDescription, t1.createdAt, t1.likesCount, t1.commentsCount, t2.first_name, t2.last_name }).ToList();
                return new JsonResult(usr);
            }
            return new JsonResult("Tweet not found");
           
        }

        [FunctionName("SearchUserTweet")]
        public IActionResult SearchUserTweet(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "user/search/{username}")] string username,ILogger log)
        {
            log.LogInformation("TweetsByUserName function triggered.");

            var tweetsByUserId = _tweetService.GetAllTweetsOfUser(username);

            var users = _userService.GetAllUsers();

            if (tweetsByUserId != null)
            {
                for (int i = 0; i < tweetsByUserId.Count; i++)
                {

                    List<TweetCommentsModel> tweetCommentsModels = new List<TweetCommentsModel>();
                    tweetCommentsModels = _tweetService.GetTweetCommentsById(tweetsByUserId[i].tweetId);
                    if (tweetCommentsModels.Count != 0)
                    {
                        if (tweetCommentsModels[0].tweetId == tweetsByUserId[i].tweetId)
                        {
                            tweetsByUserId[i].commentsCount = tweetCommentsModels.Count;
                        }
                    }
                    tweetCommentsModels = null;
                }


                var usr = (from t1 in tweetsByUserId
                           orderby t1.createdAt descending
                           join t2 in users
                           on t1.userId equals t2.userId
                           select new { t1.tweetId, t1.userId, t1.tweetTag, t1.tweetDescription, t1.createdAt, t1.likesCount, t1.commentsCount, t2.first_name, t2.last_name }).ToList();
                return new JsonResult(usr);
            }
            return new JsonResult("Tweet not found");
        }

        [FunctionName("GetAllTweets")]
        public IActionResult GetAllTweets(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "all")] HttpRequest req, ILogger log)
        {
            log.LogInformation("TweetsByUserName function triggered.");

           var allTweets = _tweetService.GetAllTweets();
            var allUsers = _userService.GetAllUsers();

            for (int i = 0; i < allTweets.Count; i++)
            {

                List<TweetCommentsModel> tweetCommentsModels = new List<TweetCommentsModel>();
                tweetCommentsModels =_tweetService.GetTweetCommentsById(allTweets[i].tweetId);
                if (tweetCommentsModels.Count != 0)
                {
                    if (tweetCommentsModels[0].tweetId == allTweets[i].tweetId)
                    {
                        allTweets[i].commentsCount = tweetCommentsModels.Count;
                    }
                }
                tweetCommentsModels = null;
            }

            for (int i = 0; i < allTweets.Count; i++)
            {
                List<TweetLikesModel> tweetLikesModel = new List<TweetLikesModel>();
                tweetLikesModel = _tweetService.GetLikesByTweetId((allTweets[i].tweetId));
                if (tweetLikesModel.Count != 0)
                {
                    if (tweetLikesModel[0].tweetId == allTweets[i].tweetId)
                    {
                        allTweets[i].likesCount = tweetLikesModel.Count;
                        allTweets[i].likeId = tweetLikesModel[0].likeId;
                    }
                }
                tweetLikesModel = null;
            }


            var usr = (from t1 in allTweets
                       orderby t1.createdAt descending
                       join t2 in allUsers
                       on t1.userId equals t2.userId
                       select new
                       {
                           t1.tweetId,
                           t1.userId,
                           t1.tweetDescription,
                           t1.tweetTag,
                           t1.createdAt,
                           t2.first_name,
                           t2.last_name,
                           t1.commentsCount,
                           t1.likesCount,
                           t1.likeId
                       }).ToList();

            if (usr != null)
            {
                return new JsonResult(usr);
            }
            return new JsonResult("Tweets not found");
        }

        [FunctionName("GetTweetLikesByTweetId")]
        public IActionResult GetTweetLikesByTweetId(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetTweetLikesByTweetId/{tweetId}")] string tweetId,ILogger log)
        {
            log.LogInformation("TweetsByUserName function triggered.");

            var tweetLikedModel = _tweetService.GetLikesByTweetId(tweetId);
            return new OkObjectResult(tweetLikedModel);
        }

        [FunctionName("LikeTweet")]
        public IActionResult LikeTweet(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "LikeTweet")][FromBody] TweetLikesModel tweetLikesModel,ILogger log)
        {
            string response = string.Empty;
            if (tweetLikesModel.liked == "like")
            {
                var likeStatus = _tweetService.LikeTweet(tweetLikesModel);
                if (likeStatus)
                {
                    response="Tweet liked successfully";
                }
            }
            else
            {
                var unlikeStatus = _tweetService.UnLike(tweetLikesModel);
                if (unlikeStatus)
                {
                    response="Tweet unliked successfully";
                }
            }
            return new OkObjectResult(response);
        }

        [FunctionName("AddComment")]
        public IActionResult ReplyTweet(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "GetTweetLikesByTweetId/{tweetId}")][FromBody] TweetCommentsModel addComment,ILogger log)
        {
            try
            {
                bool creationStatus = _tweetService.ReplyTweet(addComment);
                if (creationStatus)
                {
                    return new OkObjectResult("Tweet replied successfully");
                }
            }
            catch (Exception ex)
            {
                string message = "Meesage : " + ex.Message + " & Stacktrace: " + ex.StackTrace;
            }
            return new OkObjectResult("Error");
        }

        [FunctionName("DeleteTweet")]
        public IActionResult DeleteTweet(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "deletetweet/{id}")] string id,ILogger log)
        {
            bool status = false;
            try
            {
                status = _tweetService.DeleteTweet(id);
                if (status)
                {
                    return new OkObjectResult("Tweet deleted");
                }
                else
                {
                    return new OkObjectResult("Unable to delete tweet");
                }
            }
            catch (Exception ex)
            {
                string message = "Meesage : " + ex.Message + " & Stacktrace: " + ex.StackTrace;
            }
            return new OkObjectResult("Error");
        }

        [FunctionName("CreateTweet")]
        public IActionResult CreateTweet(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "{username}/add")][FromBody] TweetModel tweet,ILogger log)
        {
            try
            {
                bool creationStatus = _tweetService.CreateTweet(tweet);
                if (creationStatus)
                {
                    return new OkObjectResult("Tweet created successfully");
                }
            }
            catch (Exception ex)
            {
                string message = "Meesage : " + ex.Message + " & Stacktrace: " + ex.StackTrace;
            }
            return new OkObjectResult("Error");
        }

        [FunctionName("GetTweetCommentsById")]
        public IActionResult GetTweetCommentsById([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "tweetCommentsById/{tweetId}")]string tweetId,ILogger log)
        {
            List<TweetCommentsModel> tweetCommentsModels = new List<TweetCommentsModel>();
            try
            {
                tweetCommentsModels = _tweetService.GetTweetCommentsById(tweetId);
            }
            catch (Exception ex)
            {
                string message = "Meesage : " + ex.Message + " & Stacktrace: " + ex.StackTrace;
            }

            return new OkObjectResult(tweetCommentsModels);
        }


        [FunctionName("GetTweetById")]
        public IActionResult GetTweetById(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "tweetById/{tweetId}/userId/{userID}")] string tweetId, string userId,ILogger log)
        {
            TweetModel tweet = new TweetModel();

            try
            {
                tweet = _tweetService.GetTweetById(tweetId);

                List<TweetCommentsModel> tweetCommentsModels = new List<TweetCommentsModel>();
                tweetCommentsModels = _tweetService.GetTweetCommentsById(tweetId);
                if (tweetCommentsModels.Count != 0)
                {
                    if (tweetCommentsModels[0].tweetId == tweetId)
                    {
                        tweet.commentsCount = tweetCommentsModels.Count;
                    }
                }
                tweetCommentsModels = null;

                List<TweetLikesModel> tweetLikesModel = new List<TweetLikesModel>();

                tweetLikesModel = _tweetService.GetLikesByTweetId(tweetId);
                if (tweetLikesModel.Count != 0)
                {
                    if (tweetLikesModel[0].tweetId == tweetId)
                    {
                        tweet.likesCount = tweetLikesModel.Count;
                    }
                }
                tweetLikesModel = null;

                var tweetLike = _tweetService.GetLikeByTweetIdandUserID(tweetId, userId);
                if (tweetLike != null)
                {
                    tweet.likeId = tweetLike.likeId;
                }

            }
            catch (Exception ex)
            {
                string message = "Meesage : " + ex.Message + " & Stacktrace: " + ex.StackTrace;
            }

            return new OkObjectResult(tweet);
        }

        [FunctionName("GetLatestTweets")]
        public IActionResult GetLatestTweets(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "latestTweet")] ILogger log)
        {
            List<TweetModel> allTweets = new List<TweetModel>();

            List<UserModel> users = new List<UserModel>();
            try
            {
                allTweets = _tweetService.GetAllTweets();
                users = _userService.GetAllUsers();


                var usr = (from t1 in allTweets
                           orderby t1.createdAt descending
                           join t2 in users
                           on t1.userId equals t2.userId
                           select new
                           {
                               t1.tweetId,
                               t1.userId,
                               t1.tweetTag,
                               t1.tweetDescription,
                               t1.createdAt,
                               t2.first_name,
                               t2.last_name
                           }).FirstOrDefault();

                if (usr != null)
                {
                    return new OkObjectResult(usr);
                }
                return new OkObjectResult("Tweets not found");
            }
            catch (Exception ex)
            {
                string message = "Meesage : " + ex.Message + " & Stacktrace: " + ex.StackTrace;
            }
            return new OkObjectResult("Error");
        }
        
        [FunctionName("GetTweetsByUserName")]
        public IActionResult GetTweetsByUserName(
       [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{username}")] string username, ILogger log)
        {
            List<TweetModel> tweetsByUserId = new List<TweetModel>();
            try
            {
                tweetsByUserId = _tweetService.GetAllTweetsOfUser(username);
                if (tweetsByUserId != null)
                {
                    for (int i = 0; i < tweetsByUserId.Count; i++)
                    {

                        List<TweetCommentsModel> tweetCommentsModels = new List<TweetCommentsModel>();
                        tweetCommentsModels =_tweetService.GetTweetCommentsById(tweetsByUserId[i].tweetId);
                        if (tweetCommentsModels.Count != 0)
                        {
                            if (tweetCommentsModels[0].tweetId == tweetsByUserId[i].tweetId)
                            {
                                tweetsByUserId[i].commentsCount = tweetCommentsModels.Count;
                            }
                        }
                        tweetCommentsModels = null;
                    }


                    var userTweets = (from t1 in tweetsByUserId
                                      orderby t1.createdAt descending
                                      select new { t1.tweetId, t1.userId, t1.tweetTag, t1.tweetDescription, t1.createdAt, t1.commentsCount, t1.likesCount }).ToList();



                    return new OkObjectResult(userTweets);
                }
                return new OkObjectResult("Tweet not found");
            }
            catch (Exception ex)
            {
                string message = "Meesage : " + ex.Message + " & Stacktrace: " + ex.StackTrace;
            }
            return new OkObjectResult("Error");
        }

    }
}
