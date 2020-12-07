using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MapApi.Entities {
    public class Location {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("game_id")]
        public string GameId { get; set; }

        [BsonElement("location_name")]
        public string LocationName { get; set; }
    }
}

//on new game event create entry in database that maps games to adventures
//and creates an enter_location event

//we're going to listen for an entered_location event and enter it in to the database
//that enter location will need the game id

//enter_location ( gameid, adventureid, location_name )
//entered_location ( gameid, adventureid, location_name )