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
        private readonly TheFearedRepository  _theFearedRepository;
        private readonly TheFormedRepository  _theFormedRepository;
        private readonly TheHiddenRepository  _theHiddenRepository;
        private readonly TheTwistedRepository _theTwistedRepository;
        private readonly TheFearedItemRepository _theFearedItemRepository;
        private readonly TheFormedItemRepository _theFormedItemRepository;
        private readonly TheHiddenItemRepository _theHiddenItemRepository;
        private readonly TheTwistedItemRepository _theTwistedItemRepository;

        public CsvToJSONService(MemberService memberService, TheFearedRepository theFearedRepository, TheFormedRepository theFormedRepository, 
            TheHiddenRepository theHiddenRepository, TheTwistedRepository theTwistedRepository, TheFearedItemRepository theFearedItemRepository, TheFormedItemRepository theFormedItemRepository, TheHiddenItemRepository theHiddenItemRepository, TheTwistedItemRepository theTwistedItemRepository)
        {
            _memberService = memberService;
            _theFearedRepository = theFearedRepository;
            _theFormedRepository = theFormedRepository;
            _theHiddenRepository = theHiddenRepository;
            _theTwistedRepository = theTwistedRepository;
            _theFearedItemRepository = theFearedItemRepository;
            _theFormedItemRepository = theFormedItemRepository;
            _theHiddenItemRepository = theHiddenItemRepository;
            _theTwistedItemRepository = theTwistedItemRepository;
        }
        public void ReadTeamFile()
        {
            List<TeamList> _feared = _theFearedRepository.Read().ToList();
            List<TeamList> _twisted = _theTwistedRepository.Read().ToList();
            List<TeamList> _formed = _theFormedRepository.Read().ToList();
            List<TeamList> _hidden = _theHiddenRepository.Read().ToList();
            List<Member> _members = new List<Member>();

            FileStream fileStream = new FileStream("C:\\Users\\FERNANDODASILVASALGA\\source\\repos\\BPL3_Backend\\BPL3_Backend\\JSONs\\BPL3.txt", FileMode.Open);
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
                        m1.TeamName = "The Feared";
                        _feared.Add(tl);
                        _members.Add(m1);
                    }
                    if (lLine[1] != string.Empty)
                    {
                        Member m2 = new Member();
                        TeamList tl = new TeamList();
                        tl.Name = lLine[1];
                        m2.AccountName = lLine[1];
                        m2.TeamName = "The Formed";
                        _formed.Add(tl);
                        _members.Add(m2);
                    }
                    if (lLine[2] != string.Empty)
                    {
                        Member m3 = new Member();
                        TeamList tl = new TeamList();
                        tl.Name = lLine[2];
                        m3.AccountName = lLine[2];
                        m3.TeamName = "The Hidden";
                        _hidden.Add(tl);
                        _members.Add(m3);
                    }
                    if (lLine[3] != string.Empty)
                    {
                        Member m4 = new Member();
                        TeamList tl = new TeamList();
                        tl.Name = lLine[3];
                        m4.AccountName = lLine[3];
                        m4.TeamName = "The Twisted";
                        _twisted.Add(tl);
                        _members.Add(m4);
                    }
                }
                _theFearedRepository.CreateMany(_feared);
                _theFormedRepository.CreateMany(_formed);
                _theHiddenRepository.CreateMany(_hidden);
                _theTwistedRepository.CreateMany(_twisted);
                _memberService.CreateMany(_members);
            }
        }


        public void CreateItemList() 
        {
            List<Item> items = JsonSerializer.Deserialize<List<Item>>(File.ReadAllText("C:\\Users\\FERNANDODASILVASALGA\\Documents\\repos\\BPL3_Backend\\BPL3\\JSONs\\The Twisted\\TheTwistedItems.json"));
            _theFearedItemRepository.CreateMany(items);
            _theFormedItemRepository.CreateMany(items);
            _theHiddenItemRepository.CreateMany(items);
            _theTwistedItemRepository.CreateMany(items);
        }
            
    }

}
