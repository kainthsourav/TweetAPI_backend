using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace TA.Domain
{
    public class TweetLikesModel
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string likeId { get; set; }

        public string liked { get; set; }

        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string tweetId { get; set; }

        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string userId { get; set; }
        public string username { get; set; }


        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime createdAt { get; set; }
    }
}
