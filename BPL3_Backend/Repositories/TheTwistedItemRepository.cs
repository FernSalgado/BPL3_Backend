using BPL3_Backend.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace BPL3_Backend.Repositories
{
    public class TheTwistedItemRepository
    {
        private readonly IMongoCollection<Item> _member;

        public TheTwistedItemRepository(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _member = database.GetCollection<Item>("TheTwistedItems");
        }

        public Item Create(Item team)
        {
            _member.InsertOne(team);
            return team;
        }

        public List<Item> CreateMany(List<Item> members)
        {
            _member.InsertMany(members);
            return members;
        }

        public IList<Item> Read() =>
           _member.Find(m => true).ToList();

        public Item Find(string id) =>
            _member.Find(m => m.Id == id).SingleOrDefault();

        public void Update(Item member) =>
            _member.ReplaceOne(m => m.Id == member.Id, member);

        public void UpdateMany(List<Item> members)
        {
            foreach (var doc in members)
            {
                Update(doc);
            }
        }
    }
}
