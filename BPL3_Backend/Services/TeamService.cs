using BPL3_Backend.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BPL3_Backend.Services
{
    public class TeamService
    {
        private readonly IMongoCollection<Team> _teams;

        public TeamService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _teams = database.GetCollection<Team>("Team");
        }

        public Team Create(Team team)
        {
            _teams.InsertOne(team);
            return team;
        }

        public List<Team> CreateMany(List<Team> teams)
        {
            _teams.InsertMany(teams);
            return teams;
        }

        public IList<Team> Read() =>
           _teams.Find(sub => true).ToList();

        public Team Find(string name) =>
            _teams.Find(sub => sub.Name == name).SingleOrDefault();

        public void Update(Team team) =>
            _teams.ReplaceOne(sub => sub.Name == team.Name, team);
        public void UpdateMany(List<Team> team) 
        {
            foreach (var doc in team)
            {
                Update(doc);
            }
        }



    }
}
