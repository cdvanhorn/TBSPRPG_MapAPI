using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MapApi.Entities {
    public class Game {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("adventure_id")]
        public string AdventureId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("user_id")]
        public string UserId { get; set; }
    }
}