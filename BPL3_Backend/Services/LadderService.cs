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
        private readonly SheetService _sheetService;
        public LadderService(MemberService memberService, TeamService teamService, SheetService sheetService)
        {
            _memberService = memberService;
            _teamService = teamService;
            _sheetService = sheetService;
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
                var url = ($"https://www.pathofexile.com/api/ladders?offset={count}&limit=200&id=Rebellion%20BPL%20(PL16111)&type=league&realm=pc&_=1612548934164&type=league&realm=pc&_=1612548934164");
                var streamTask = client.GetStringAsync(url);
                var aaa = streamTask.Result;
                aaa = aaa.Replace("\"class\"", "\"Class\"");
                dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(aaa);
                if (json.entries.Count == 0) count = total; ;
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
                                if (item.character.level > m.Level)
                                {
                                    m.CharacterName = item.character.name;
                                    m.Class = item.character.Class;
                                    m.Rank = item.rank;
                                }
                                if (item.character.depth != null && item.character.depth.solo > m.Delve)
                                    m.Delve = item.character.depth.solo;
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
                List<Member> team1Members = members.Where(m => m.TeamName == "Order").ToList();
                List<Member> team2Members = members.Where(m => m.TeamName == "Chaos").ToList();
                List<Member> team3Members = members.Where(m => m.TeamName == "Ruin").ToList();
                Team team1 = teams.Where(t => t.Name == "Order").FirstOrDefault();
                Team team2 = teams.Where(t => t.Name == "Chaos").FirstOrDefault();
                Team team3 = teams.Where(t => t.Name == "Ruin").FirstOrDefault();
                List<int> points = new List<int>();
                points.AddRange(CalcPoints(team1Members));
                points.AddRange(CalcPoints(team2Members));
                points.AddRange(CalcPoints(team3Members));
                var gemPoints = _sheetService.getSheet();
                team1.LevelPoints = points[0];
                team1.DelvePoints = points[1];
                team1.GemPoints = gemPoints[0];
                team1.TotalPoints = team1.LevelPoints + team1.DelvePoints + team1.SetPoints + team1.GemPoints;
                team2.LevelPoints = points[2];
                team2.DelvePoints = points[3];
                team2.GemPoints = gemPoints[1];
                team2.TotalPoints = team2.LevelPoints + team2.DelvePoints + team2.SetPoints + team2.GemPoints;
                team3.LevelPoints = points[4];
                team3.DelvePoints = points[5];
                team3.GemPoints = gemPoints[2];
                team3.TotalPoints = team3.LevelPoints + team3.DelvePoints + team3.SetPoints + team3.GemPoints;
                _teamService.UpdateMany(new List<Team> {team2, team1,team3 });
            }
        }
        private static List<int> CalcPoints(List<Member> members)
        {
            List<int> points = new List<int>();
            points.Add(0);
            points.Add(0);
            foreach (var item in members)
            {
                if (!item.Class.Contains("Invalid")) 
                {
                    points[0] += CalcTeamLevelPoints(item.Level);
                    points[1] += CalcTeamDelvePoints(item.Delve);
                 }
            }
            return points;
        }
        private static int CalcTeamLevelPoints(int level)
        {
            int points = 0;

            if (level <= 90)
            {
                points += level;
            }
            else
            {
                if (level > 95) 
                {
                    points += 105;
                }
                else
                {
                    var multi = level - 90;
                    points += 90 + (multi * 3);
                }
            }
            if (level >= 90) points += 10 + 85;
            else points += ((int)level / 5)*5;
            
            return points;
        }
        private static int CalcTeamDelvePoints(int delve)
        {
            int points = 0;
            if (delve >= 10 && delve <= 250) 
            {
                points += ((int)delve / 10) * 15;
            }
            if (delve >= 251 && delve <= 400)
            {
                var multi = delve - 250;
                points += 375 + (multi * 7); //depth 250 and 251+

                if (delve >= 400) points += 200;
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
