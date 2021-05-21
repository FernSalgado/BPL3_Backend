using BPL3_Backend.Models;
using BPL3_Backend.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace BPL3_Backend.Services
{

    public interface ICsvToJSONService
    {
        public void ReadTeamFile();
        public void CreateItemList();
    }
    public class CsvToJSONService : ICsvToJSONService
    {
        public static readonly string filePathBase = "C:\\Users\\FERNANDODASILVASALGA\\Documents\\repos\\BPL3_Backend\\BPL3\\JSONs\\";
        private readonly MemberService _memberService;
        private readonly Team1Repository _team1Repository;
        private readonly Team2Repository _team2Repository;
        private readonly Team3Repository _team3Repository;
        private readonly Team1ItemRepository _team1ItemRepository;
        private readonly Team2ItemRepository _team2ItemRepository;
        private readonly Team3ItemRepository _team3ItemRepository;

        public CsvToJSONService(MemberService memberService, Team1Repository team1Repository, Team3Repository theFormedRepository, 
             Team2Repository team2Repository, Team1ItemRepository team1ItemRepository, Team3ItemRepository team3ItemRepository, Team2ItemRepository team2ItemRepository)
        {
            _memberService = memberService;
            _team1Repository = team1Repository;
            _team2Repository = team2Repository;
            _team3Repository = theFormedRepository;
            _team1ItemRepository = team1ItemRepository;
            _team2ItemRepository = team2ItemRepository;
            _team3ItemRepository = team3ItemRepository;
        }
        public void ReadTeamFile()
        {
            List<TeamList> _team1 = _team1Repository.Read().ToList();
            List<TeamList> _team2 = _team2Repository.Read().ToList();
            List<TeamList> _team3 = _team3Repository.Read().ToList();
            List<Member> _members = new List<Member>();

            FileStream fileStream = new FileStream("C:\\Users\\Fernando\\Documents\\repo\\bpl3_backend\\BPL3_Backend\\JSONs\\bpl.txt", FileMode.Open);
            using (StreamReader reader = new StreamReader(fileStream))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    List<string> lLine = new List<string>();
                    lLine.AddRange(line.Split(","));
                    if (lLine[0] != string.Empty)
                    {
                        Member m1 = new Member();
                        TeamList tl = new TeamList();
                        tl.Name = lLine[0];
                        m1.AccountName = lLine[0];
                        m1.TeamName = "Order";
                        _team1.Add(tl);
                        _members.Add(m1);
                    }
                    if (lLine[1] != string.Empty)
                    {
                        Member m2 = new Member();
                        TeamList tl = new TeamList();
                        tl.Name = lLine[1];
                        m2.AccountName = lLine[1];
                        m2.TeamName = "Chaos";
                        _team2.Add(tl);
                        _members.Add(m2);
                    }
                    if (lLine[2] != string.Empty)
                    {
                        Member m3 = new Member();
                        TeamList tl = new TeamList();
                        tl.Name = lLine[2];
                        m3.AccountName = lLine[2];
                        m3.TeamName = "Ruin";
                        _team3.Add(tl);
                        _members.Add(m3);
                    }
                }
                _team1Repository.CreateMany(_team1);
                _team2Repository.CreateMany(_team2);
                _team3Repository.CreateMany(_team3);
                _memberService.CreateMany(_members);
            }
        }


        public void CreateItemList() 
        {
            List<Item> items = JsonSerializer.Deserialize<List<Item>>(File.ReadAllText("C:\\Users\\Fernando\\Documents\\repo\\bpl3_backend\\BPL3_Backend\\JSONs\\Items.json"));
            _team1ItemRepository.CreateMany(items);
            _team3ItemRepository.CreateMany(items);
            _team2ItemRepository.CreateMany(items);
        }
            
    }

}
