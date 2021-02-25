using BPL3_Backend.Models;
using BPL3_Backend.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace BPL3_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeamController : Controller
    {
        private readonly TeamService _teamService;
        private string filePathBase;

        public TeamController(TeamService teamService)
        {
            _teamService = teamService;
        }
        [HttpGet]
        public ActionResult<Team> get() 
        {
            IList<Team> listTeam = null;
            try {
                listTeam = _teamService.Read();
            } catch (Exception ex) {
                return BadRequest();
            }
            if (listTeam.Any())
                return Ok(listTeam);
            else
                return NoContent();
        }


        [HttpPost]
        public ActionResult<Team> CreateMany(List<Team> teams)
        {
            try
            {
                _teamService.UpdateMany(teams);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
            return Ok();
        }
    }
}
