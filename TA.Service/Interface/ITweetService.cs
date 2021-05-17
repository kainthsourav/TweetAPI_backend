using System;
using System.Collections.Generic;
using System.Text;
using TA.Domain;

namespace TA.Service.Interface
{
    public interface ITweetService
    {
        List<TweetModel> GetAllTweets();
        List<TweetModel> GetAllTweetsOfUser(string username);

        List<TweetModel> GetAllTweetSearchUser(string username);
        List<TweetCommentsModel> GetAllComments();
        TweetModel GetTweetById(string tweetId);
        List<TweetCommentsModel> GetTweetCommentsById(string tweetId);

        bool CreateTweet(TweetModel tweet);
        bool UpdateTweet(TweetModel tweetModel);
        bool DeleteTweet(string tweetId);
        TweetLikesModel GetLikeByTweetIdandUserID(string tweetId, string userId);
        bool LikeTweet(TweetLikesModel tweetLikes);
        bool UnLike(TweetLikesModel tweetLike);

        List<TweetLikesModel> GetAllLikes();
        List<TweetLikesModel> GetLikesByTweetId(string tweetId);
        bool ReplyTweet(TweetCommentsModel tweetModel);
    }
}
