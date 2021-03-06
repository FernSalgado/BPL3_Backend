﻿using AngleSharp;
using BPL3_Backend.Models;
using BPL3_Backend.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace BPL3_Backend.Services
{
    public class WebScrappingService
    {
        private readonly Team1ItemRepository _team1ItemRepository;
        private readonly Team3ItemRepository _team3ItemRepository;
        private readonly Team2ItemRepository _team2ItemRepository;
        private readonly TeamService _teamService;
        private readonly LadderService _ladderService;
        private IBrowsingContext context { get; set; }

        public WebScrappingService(TeamService teamService, Team1Repository theFearedRepository, Team3Repository theFormedRepository,  Team2Repository theTwistedRepository,
            Team1ItemRepository team1ItemRepository, Team3ItemRepository team3ItemRepository, Team2ItemRepository team2ItemRepository, LadderService ladderService)
        {
            var config = Configuration.Default.WithDefaultLoader();
            context = BrowsingContext.New(config);
            _team1ItemRepository = team1ItemRepository;
            _team2ItemRepository = team2ItemRepository;
            _team3ItemRepository = team3ItemRepository;
            _teamService = teamService;
            _ladderService = ladderService;
        }

        public async Task ScrapperItems() 
        {
            var team1Items = _team1ItemRepository.Read().ToList();
            var team1 = _teamService.Find("Order");

            var team2Items = _team2ItemRepository.Read().ToList();
            var team2 = _teamService.Find("Chaos");

            var team3Items = _team3ItemRepository.Read().ToList();
            var team3 = _teamService.Find("Ruin");

            TeamItem Team1 = await GetItems(new TeamItem { Items = team1Items, Team = team1 });
            TeamItem Team2 = await GetItems(new TeamItem { Items = team2Items, Team = team2 });
            TeamItem Team3 = await GetItems(new TeamItem { Items = team3Items, Team = team3 });
            _team1ItemRepository.UpdateMany(Team1.Items);
            _team2ItemRepository.UpdateMany(Team2.Items);
            _team3ItemRepository.UpdateMany(Team3.Items);

           _ladderService.GetLadder(new List<Team> { Team1.Team, Team2.Team, Team3.Team });

        }

        public async Task<TeamItem> GetItems(TeamItem teamItem)
        {
            teamItem.Team.SetPoints = 0;
            foreach (var item in teamItem.Items)
            {
                item.Obtained = "False";
            }
            var encoderSettings = new TextEncoderSettings();
            encoderSettings.AllowRange(UnicodeRanges.All);
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };
            List<string> _allItems = JsonSerializer.Deserialize<List<string>>(File.ReadAllText("JSONs\\ItemList.json"), options);
            _allItems.Add("Doppelgänger Guise");
            List<string> _allSets = JsonSerializer.Deserialize<List<string>>(File.ReadAllText("JSONs\\SetList.json" ),options);
            List<string> _items = new List<string>();
            if (teamItem.Team.StashUrl == String.Empty) return teamItem;
            for (int i = 1; i < 21; i++)
            {
                var url = $"{teamItem.Team.StashUrl}/{i}";
                var document = await context.OpenAsync(url);
                var items = document.QuerySelectorAll(".owned");
                foreach (var item in items)
                {
                    var _item = item.FirstElementChild.FirstElementChild.InnerHtml;
                    if (_item == "Impresence")
                    {
                        var count = 0;
                        var node = item.PreviousElementSibling;
                        while (node != null && node.FirstElementChild.FirstElementChild.InnerHtml == "Impresence")
                        {
                            node = node.PreviousElementSibling;
                            count++;
                        }
                        if (count == 0) _item = "Impresence (Cold)";
                        if (count == 1) _item = "Impresence (Fire)";
                        if (count == 2) _item = "Impresence (Lightning)";
                        if (count == 3) _item = "Impresence (Phys)";
                        if (count == 4) _item = "Impresence (Chaos)";
                    }
                    if (_item == "Volkuur's Guidance")
                    {
                        var count = 0;
                        var node = item.PreviousElementSibling;
                        while (node != null && node.FirstElementChild.FirstElementChild.InnerHtml == "Volkuur's Guidance")
                        {
                            node = node.PreviousElementSibling;
                            count++;
                        }
                        if (count == 0) _item = "Volkuur's Guidance (Fire)";
                        if (count == 1) _item = "Volkuur's Guidance (Cold)";
                        if (count == 2) _item = "Volkuur's Guidance (Lightning)";
                    }
                    _items.Add(_item);
                }
            }
            var results = _items.Select(i => i).ToList().Intersect(_allItems.Select(i => i).ToList()).ToList();
            foreach (var item in results)
            {
                Item i = teamItem.Items.Where(i => i.Name.ToLower() == item.ToLower() && i.Obtained == "False").FirstOrDefault();
                if (i != null)
                {
                    i.Obtained = "True";
                    if (i.SetName != "Labyrinth Jewel Set") teamItem.Team.SetPoints += 250;
                }
            }
            int cSets = 0;
            string sets = "";
            foreach (var item in _allSets)
            {
                var set = teamItem.Items.Where(i => i.SetName == item && i.Obtained == "False").ToList();
                if (set.Count == 0) 
                {
                    teamItem.Team.SetPoints += 500;
                    cSets += 1;
                    sets += item + ";";
                } 
            }
            teamItem.Team.CompletedSets = sets;
            teamItem.Team.ObtainedSets = cSets;
            teamItem.Team.ObtainedUniques = results.Count();
            return teamItem;
        }
    }
}
