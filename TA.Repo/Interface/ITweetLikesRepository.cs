using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using TA.Domain;

namespace TA.Repo.Interface
{
    public interface ITweetLikesRepository
    {
        List<TweetLikesModel> FindAll();
        List<TweetLikesModel> FindAllByCondition(Expression<Func<TweetLikesModel, bool>> expression);
        TweetLikesModel FindByCondition(Expression<Func<TweetLikesModel, bool>> expression);
        bool Create(TweetLikesModel tweetLike);
        bool Delete(TweetLikesModel unlike);

        //  bool Update(TweetLikesModel data);
    }
}
