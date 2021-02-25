using BPL3_Backend.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BPL3_Backend.Services
{
    public class MemberService
    {
        private readonly IMongoCollection<Member> _member;

        public MemberService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _member = database.GetCollection<Member>("Members");
        }

        public Member Create(Member team)
        {
            _member.InsertOne(team);
            return team;
        }

        public List<Member> CreateMany(List<Member> members)
        {
            _member.InsertMany(members);
            return members;
        }

        public IList<Member> Read() =>
           _member.Find(m => true).ToList();

        public Member Find(string id) =>
            _member.Find(m => m.Id == id).SingleOrDefault();

        public void Update(Member member) =>
            _member.ReplaceOne(m => m.Id == member.Id, member);

        public void UpdateMany(List<Member> members)
        {
            foreach (var doc in members)
            {
                Update(doc);
            }
        }
    }
}
