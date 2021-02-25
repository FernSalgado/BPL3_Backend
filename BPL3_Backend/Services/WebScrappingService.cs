using AngleSharp;
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
    public class WebScrappingService
    {
        private readonly TheFearedItemRepository _theFearedItemRepository;
        private readonly TheFormedItemRepository _theFormedItemRepository;
        private readonly TheHiddenItemRepository _theHiddenItemRepository;
        private readonly TheTwistedItemRepository _theTwistedItemRepository;
        private readonly TeamService _teamService;
        private readonly LadderService _ladderService;
        private IBrowsingContext context { get; set; }

        public WebScrappingService(TeamService teamService, TheFearedRepository theFearedRepository, TheFormedRepository theFormedRepository, TheHiddenRepository theHiddenRepository, TheTwistedRepository theTwistedRepository, TheFearedItemRepository theFearedItemRepository, TheFormedItemRepository theFormedItemRepository, TheHiddenItemRepository theHiddenItemRepository, TheTwistedItemRepository theTwistedItemRepository, LadderService ladderService)
        {
            var config = Configuration.Default.WithDefaultLoader();
            context = BrowsingContext.New(config);
            _theFearedItemRepository = theFearedItemRepository;
            _theFormedItemRepository = theFormedItemRepository;
            _theHiddenItemRepository = theHiddenItemRepository;
            _theTwistedItemRepository = theTwistedItemRepository;
            _teamService = teamService;
            _ladderService = ladderService;
        }

        public async Task ScrapperItems() 
        {
            var fearedItems = _theFearedItemRepository.Read().ToList();
            var feared = _teamService.Find("The Feared");

            var twistedItems = _theTwistedItemRepository.Read().ToList();
            var twisted = _teamService.Find("The Twisted");

            var formedItems = _theFormedItemRepository.Read().ToList();
            var formed = _teamService.Find("The Formed");

            var hiddenItems = _theHiddenItemRepository.Read().ToList();
            var hidden = _teamService.Find("The Hidden");

            TeamItem TheFeared = await GetItems(new TeamItem { Items = fearedItems, Team = feared });
            TeamItem TheTwisted = await GetItems(new TeamItem { Items = twistedItems, Team = twisted });
            TeamItem TheFormed = await GetItems(new TeamItem { Items = formedItems, Team = formed });
            TeamItem TheHidden = await GetItems(new TeamItem { Items = hiddenItems, Team = hidden });
            _theFearedItemRepository.UpdateMany(TheFeared.Items);
            _theTwistedItemRepository.UpdateMany(TheTwisted.Items);
            _theHiddenItemRepository.UpdateMany(TheHidden.Items);
            _theFormedItemRepository.UpdateMany(TheFormed.Items);
            _ladderService.GetLadder(new List<Team> { TheFeared.Team, TheTwisted.Team, TheFormed.Team, TheHidden.Team });
        }

        public async Task<TeamItem> GetItems(TeamItem teamItem)
        {
            foreach (var item in teamItem.Items)
            {
                item.Obtained = "False";
            }
            List<string> _allItems = JsonSerializer.Deserialize<List<string>>(File.ReadAllText("C:\\Users\\FERNANDODASILVASALGA\\source\\repos\\BPL3\\BPL3\\JSONs\\ItemList.json"));
            List<string> _items = new List<string>();
            for (int i = 1; i < 21; i++)
            {
                if (i != 5 && i != 19)
                {
                    var url = $"{teamItem.Team.StashUrl}/{i}";
                    var document = await context.OpenAsync(url);
                    var items = document.QuerySelectorAll(".owned");
                    foreach (var item in items)
                    {
                        _items.Add(item.FirstElementChild.FirstElementChild.InnerHtml);
                    }
                }
            }
            var results = _items.Select(i => i).ToList().Intersect(_allItems.Select(i => i).ToList()).ToList();
            foreach (var item in results)
            {
                Item i = teamItem.Items.Where(i => i.Name == item && i.Obtained == "False").FirstOrDefault();
                if (i != null)
                {
                    i.Obtained = "True";
                    var set = teamItem.Items.Where(it => it.SetName == i.SetName && it.Obtained == "False").ToList();
                    if (set.Count == 0) 
                    {
                        if (i.SetName != "Xoph's Set" && i.SetName != "Esh's Set" && i.SetName != "Tul's Set" && i.SetName != "Uul-Netol's Set" && i.SetName != "Chayula's Set")
                        {
                            if (i.SetName == "Shaper's Set") teamItem.Team.SetPoints += 200;
                            if (i.SetName == "Elder's Set") teamItem.Team.SetPoints += 200;
                            if (i.SetName == "Uber Elder's Set") teamItem.Team.SetPoints += 550;
                            if (i.SetName == "Maven's Set") teamItem.Team.SetPoints += 550;
                            if (i.SetName == "Synthesis Set") teamItem.Team.SetPoints += 425;
                            if (i.SetName == "Uber Atziri's Set") teamItem.Team.SetPoints += 425;
                        }
                        else 
                        {
                            var xoph = teamItem.Items.Where(it => it.SetName == "Xoph's Set" && it.Obtained == "False").ToList();
                            var esh = teamItem.Items.Where(it => it.SetName == "Esh's Set" && it.Obtained == "False").ToList();
                            var uul = teamItem.Items.Where(it => it.SetName == "Uul-Netol's Set" && it.Obtained == "False").ToList();
                            var tul = teamItem.Items.Where(it => it.SetName == "Tul's Set" && it.Obtained == "False").ToList();
                            var chay = teamItem.Items.Where(it => it.SetName == "Chayula's Set" && it.Obtained == "False").ToList();
                            var count = xoph.Count + esh.Count + uul.Count + tul.Count + chay.Count;
                            if (count == 0)
                            {
                                teamItem.Team.SetPoints += 425;
                            }
                        }
                    }
                }
            }
            return teamItem;
        }
    }
}
