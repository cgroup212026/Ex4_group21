using System.Data.SqlClient;
using System.Data;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Task1.Models;
using System.Collections.Generic;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Task1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        // GET: api/<GamesController>
        [HttpGet]
        public IEnumerable<Game> Get()
        {
            Game game = new Game();
            return game.read();
        }

        // GET api/<GamesController>/5
        [HttpGet("{name}")]
        public List<Game> Get(string name)
        {
            Game g = new Game();
            return g.GetByName(name);
        }

        // GET api/<GamesController>/5
        [HttpGet("GetAllTags")]
        public List<string> GetAllTags()
        {
            Game g = new Game();
            return g.AllTags();
        }


        // GET api/<GamesController>/5
        [HttpGet("GetByTags")]
        public List<Game> GetGamesByTags(string tags)
        {
            Game g = new Game();
            return g.GameByTags(tags);
        }

        [HttpGet("recommendations/{userId}")]
        public List<Game> GetRecommendations(int userId)
        {
            Game game = new Game();
            return game.GetRecommendedGames(userId);
        }

        // POST api/<GamesController>
        [HttpPost]
        public int Post([FromBody] Game game)
        {
            if(game.insert(game))
            {
                return 1;
            }
            return 0;
        }

        // PUT api/<GamesController>/5
        [HttpPut("{id}")]
        public int Put(int id, [FromBody] Game game)
        {
            return Game.UpdateGame(id, game);
        }

        // DELETE api/<GamesController>/5
        [HttpDelete]
        public int Delete(int id)
        {
            Game g = new Game();
            return g.delete(id);
        }


        // POST api/<UploadController>
        [HttpPost("UploadGames")]
        public async Task<IActionResult> UploadGames([FromForm] IFormFile file)
        {
            Game game = new Game();

            string result = await game.UploadGames(file);

            if (result.StartsWith("ERROR:"))
            {
                return BadRequest(result.Replace("ERROR:", ""));
            }

            return Ok(result);
        }
        //[HttpPost("UploadGames")]
        //public async Task<IActionResult> UploadGames([FromForm]IFormFile file)
        //{
        //    // 1. Check if the file is valid
        //    if (file == null || file.Length == 0)
        //    {
        //        return BadRequest("File is empty or missing.");
        //    }

        //    // 2. We need a string to hold the JSON text
        //    string jsonContent = "";

        //    // 3. Open the file and read it from top to bottom
        //    using (var streamReader = new StreamReader(file.OpenReadStream()))
        //    {
        //        jsonContent = await streamReader.ReadToEndAsync();
        //    }

        //    // 4. Set up rules for the JSON reader. 
        //    // This rule is CRITICAL: JSON uses "gameName", C# uses "GameName". 
        //    // This tells C# to ignore uppercase/lowercase differences so it matches perfectly.
        //    var options = new JsonSerializerOptions
        //    {
        //        PropertyNameCaseInsensitive = true
        //    };

        //    try
        //    {
        //        // 5. THE CONVERSION: Turn the giant string into a List<Game>
        //        List<Game> gamesList = JsonSerializer.Deserialize<List<Game>>(jsonContent, options);


        //        DBservices db = new DBservices();
        //        foreach(Game g in gamesList) 
        //        {
        //            db.InsertAGame(g);
        //        }

        //        return Ok($"Success! Converted file into a list of {gamesList.Count} games.");
        //    }
        //    catch (JsonException ex)
        //    {
        //        // If the JSON is formatted wrong (missing a bracket, wrong data type), it crashes here.
        //        return BadRequest($"Failed to convert JSON. Error: {ex.Message}");
        //    }
        //}

    }
}
