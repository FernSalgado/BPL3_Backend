using BPL3_Backend.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace BPL3_Backend.Repositories
{
    public class TheTwistedRepository
    {
        private readonly IMongoCollection<TeamList> _member;

        public TheTwistedRepository(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _member = database.GetCollection<TeamList>("TheTwisted");
        }

        public TeamList Create(TeamList team)
        {
            _member.InsertOne(team);
            return team;
        }

        public List<TeamList> CreateMany(List<TeamList> members)
        {
            _member.InsertMany(members);
            return members;
        }

        public IList<TeamList> Read() =>
           _member.Find(m => true).ToList();

        public TeamList Find(string id) =>
            _member.Find(m => m.Id == id).SingleOrDefault();

        public void Update(TeamList member) =>
            _member.ReplaceOne(m => m.Id == member.Id, member);

        public void UpdateMany(List<TeamList> members)
        {
            foreach (var doc in members)
            {
                Update(doc);
            }
        }
    }
}
