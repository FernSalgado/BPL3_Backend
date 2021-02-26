using BPL3_Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BPL3_Backend.Services
{
    public class LadderService
    {
        private readonly MemberService _memberService;
        private readonly TeamService _teamService;
        public LadderService(MemberService memberService, TeamService teamService)
        {
            _memberService = memberService;
            _teamService = teamService;
        }
        private static readonly HttpClient client = new HttpClient();
        public void GetLadder(List<Team> t)
        {
            List<Member> members = new List<Member>();
            List<Team> teams = new List<Team>();
            members = _memberService.Read().ToList();
            teams = t;
            var total = members.Count();
            var count = 0;
            var update = false;
            do
            {
                client.DefaultRequestHeaders.Accept.Clear();
                var url = ($"https://www.pathofexile.com/api/ladders?offset={count}&limit=200&id=Badgers%20Invitational%20(PL13903)&type=league&realm=pc&_=1612548934164");
                var streamTask = client.GetStringAsync(url);
                var aaa = streamTask.Result;
                aaa = aaa.Replace("\"class\"", "\"Class\"");
                dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(aaa);
                foreach (var item in json.entries)
                {
                    update = true;
                    string name = item.account.name;
                    Member m = members.Where(m => m.AccountName.ToLower() == name.ToLower()).FirstOrDefault();
                    if (m != null)
                    {
                        var team = teams.Where(t => t.Name == m.TeamName).FirstOrDefault();
                        if (m.CharacterName == null)
                        {
                            m.CharacterName = item.character.name;
                            string c = item.character.Class;
                            m.Class = IsClassValid(team.AllowedClasses, c) ? c : $"Invalid Class ({item.character.Class})";
                            m.Rank = item.rank;
                        }
                        else
                        {
                            if (m.CharacterName != item.character.name.ToString())
                            {
                                m.CharacterName = item.character.level > m.Level ? item.character.name : m.CharacterName;
                                if (m.CharacterName != item.character.name.ToString()) continue;
                            }
                            m.Class = IsClassValid(team.AllowedClasses, item.character.Class.ToString()) ? item.character.Class : $"Invalid Class ({item.character.Class})";
                        }
                        m.Level = item.character.level;
                        m.Rank = item.rank;
                        m.Delve = item.character.depth != null ? item.character.depth.solo : 0;
                    }
                }
                count += 200;
            } while (count < total);
            if (update) 
            {
                members.RemoveAll(me => me.CharacterName == null);
                _memberService.UpdateMany(members);
                List<Member> theTwistedMembers = members.Where(m => m.TeamName == "The Twisted").ToList();
                List<Member> theFearedMembers = members.Where(m => m.TeamName == "The Feared").ToList();
                List<Member> theHiddenMembers = members.Where(m => m.TeamName == "The Hidden").ToList();
                List<Member> theFormedMembers = members.Where(m => m.TeamName == "The Formed").ToList();
                Team theTwisted = teams.Where(t => t.Name == "The Twisted").FirstOrDefault();
                Team theFeared = teams.Where(t => t.Name == "The Feared").FirstOrDefault();
                Team theHidden = teams.Where(t => t.Name == "The Hidden").FirstOrDefault();
                Team theFormed = teams.Where(t => t.Name == "The Formed").FirstOrDefault();
                List<int> points = new List<int>();
                points.AddRange(CalcPoints(theTwistedMembers));
                points.AddRange(CalcPoints(theFearedMembers));
                points.AddRange(CalcPoints(theHiddenMembers));
                points.AddRange(CalcPoints(theFormedMembers));
                theTwisted.LevelPoints = points[0];
                theTwisted.DelvePoints = points[1];
                theTwisted.TotalPoints = theTwisted.LevelPoints + theTwisted.DelvePoints + theTwisted.SetPoints;
                theFeared.LevelPoints = points[2];
                theFeared.DelvePoints = points[3];
                theFeared.TotalPoints = theFeared.LevelPoints + theFeared.DelvePoints + theFeared.SetPoints;
                theHidden.LevelPoints = points[4];
                theHidden.DelvePoints = points[5];
                theHidden.TotalPoints = theHidden.LevelPoints + theHidden.DelvePoints + theHidden.SetPoints;
                theFormed.LevelPoints = points[6];
                theFormed.DelvePoints = points[7];
                theFormed.TotalPoints = theFormed.LevelPoints + theFormed.DelvePoints + theFormed.SetPoints;
                _teamService.UpdateMany(new List<Team> {theFeared, theTwisted, theFormed,theHidden });
            }
        }
        private static List<int> CalcPoints(List<Member> members)
        {
            List<int> points = new List<int>();
            points.Add(0);
            points.Add(0);
            foreach (var item in members)
            {
                points[0] += CalcTeamLevelPoints(item.Level);
                points[1] += CalcTeamDelvePoints(item.Delve);
            }
            return points;
        }
        private static int CalcTeamLevelPoints(int level)
        {
            int points = 0;

            if (level <= 95)
            {
                points += level;
            }
            else
            {
                points += (level + ((level - 95) * 5));
            }

            if (level >= 10 && level <= 80)
            {
                points += (level / 10) * 2;
            }
            else
            {
                if (level >= 85) points += 5;
                if (level >= 90) points += 10;
                if (level >= 55) points += 15;
            }
            return points;
        }
        private static int CalcTeamDelvePoints(int delve)
        {
            int points = 0;
            int count = 100;
            if (delve >= 100)
            {
                while (count <= delve && count <= 500)
                {
                    points += 4;
                    count += 25;
                }
                if (delve >= 300 && delve <= 500)
                {
                    points += ((delve - 200) / 100) * 4;
                }
                if (delve > 600)
                {
                    points += 12;
                    points += (delve - 500) / 10;
                }
            }
            return points;
        }
        public static bool IsClassValid(List<string> classes, string Class)
        {
            try
            {
                return classes.Contains(Class);
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
