using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BPL3_Backend.Models
{
    public class Member
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string CharacterName { get; set; }
        public string AccountName { get; set; }
        public string TeamName { get; set; }
        public int Level { get; set; }
        public int Delve { get; set; }
        public int Rank { get; set; }
        public string Class { get; set; }
    }
}
