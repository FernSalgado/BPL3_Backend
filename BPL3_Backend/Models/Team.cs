using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BPL3_Backend.Models
{
    public class Team
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Leader { get; set; }
        public int TotalPoints { get; set; }
        public int LevelPoints { get; set; }
        public int DelvePoints { get; set; }
        public int SetPoints { get; set; }
        public int BossPoints { get; set; }
        public int GemPoints { get; set; }
        public string StashUrl { get; set; }
        public List<string> AllowedClasses { get; set; }
    }
}
