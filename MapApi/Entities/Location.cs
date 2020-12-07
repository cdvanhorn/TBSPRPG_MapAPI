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

//final notes
//Listen for new_game events
//on new game event, build the aggregrate
//check if the event id prefixed with this service id is in the list of event ids in the aggregrate
//convert it to a MapApi.Game object
//write the game to the database game collection for this service
//look up the initial location for the given adventure
//create an enter_location event that contains this service id plus the new_game event id

//Listen for entered_location event
//build the game aggregrate
//check if the entered_location event id prefixed with this service id is in the list of event ids in the aggregrate
//convert it ot a MapApi.Game object
//write to the location collection in the map database