using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using TA.Domain;

namespace TA.Repo.Interface
{
    public interface ITweetCommentsRepository
    {
        List<TweetCommentsModel> FindAll();
        List<TweetCommentsModel> FindAllByCondition(string tweetId);
        TweetCommentsModel FindByCondition(Expression<Func<TweetCommentsModel, bool>> expression);
        bool Create(TweetCommentsModel tweetComment);
        bool Update(TweetCommentsModel tweetComment);
        bool Delete(string tweetCommentId);
    }
}
