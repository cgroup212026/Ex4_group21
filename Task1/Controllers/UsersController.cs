using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using Task1.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Task1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        // GET: api/<UsersController>
        [HttpGet]
        public List<User> Get()
        {
            User user = new User();
            return user.read();
        }

        // GET api/<UsersController>/5
        [HttpPost("Login")]
        public IActionResult Login([FromBody] System.Text.Json.JsonElement loginData)
        {
            User user = new User();

            string email = loginData.GetProperty("Email").GetString();
            string password = loginData.GetProperty("Password").GetString();

            List<string> value = user.login(email, password);

            if (value != null && value.Count >= 2)
            {
                return Ok(new
                {
                    userName = value[0],
                    idNum = value[1]
                });
            }

            return Unauthorized("Invalid email or password");
        }

        // GET: api/<UsersController>
        [HttpGet("GetUserGames")]
        public List<Game> Get(int id)
        {
            User u = new User();
            return u.GetUserGames(id);
        }

        // POST api/<UsersController>
        [HttpPost]
        public IActionResult Post([FromBody] User user)
        {
            User u = new User();
            int id = u.insert(user);
            if(id != 0)
            {
                return Ok(new { Useridtoreturn = id});

            }
            return NotFound();
        }

        // POST api/<UsersController>
        [HttpPost("InsertToUserGameTable/{userId}/{gameId}")]
        public int Post(int userid, int gameid)
        {
            User u = new User();
            return u.insertgametbl(userid, gameid);
        }

        // PUT api/<UsersController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, User user)
        {
            User u = new User();
            string val = u.Updatebyid(id, user);
            return Ok(new { userName = val});
        }

        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        public int Delete(int id)
        {
            User u = new User();
            return u.DeleteUser(id);

        }

        // DELETE api/<GamesController>/5
        [HttpDelete("DeleteUserGame/{userId}/{gameId}")]
        public int DeleteUserGame(int userId, int gameId)
        {
            User u = new User();
            return u.DeleteUserGames(userId, gameId);
        }
    }
}
