using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Task1.Controllers;

namespace Task1.Models
{
    public class Game
    {
        int id;
        string name;
        string steamURL;
        string img;
        string releaseDate;
        string reviewSummary;
        double price;
        List<string> tags;
        bool windows;
        bool mac;
        bool linux;
        public static List<Game> gamesList = new List<Game>();
        DBservices dbservices = new DBservices();

        public Game()
        {

        }
        public Game(int id, string name, string steamURL, string img, string releaseDate, string reviewSummary, double price, List<string> tags, bool windows, bool mac, bool linux)
        {
            Id = id;
            Name = name;
            SteamURL = steamURL;
            Img = img;
            ReleaseDate = releaseDate;
            ReviewSummary = reviewSummary;
            Price = price;
            Tags = tags;
            Windows = windows;
            Mac = mac;
            Linux = linux;
        }

        public int Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public string SteamURL { get => steamURL; set => steamURL = value; }

        [JsonPropertyName("capsuleImage")]
        public string Img { get => img; set => img = value; }
        public string ReleaseDate { get => releaseDate; set => releaseDate = value; }
        public string ReviewSummary { get => reviewSummary; set => reviewSummary = value; }
        public double Price { get => price; set => price = value; }
        public List<string> Tags { get => tags; set => tags = value; }
        public bool Windows { get => windows; set => windows = value; }
        public bool Mac { get => mac; set => mac = value; }
        public bool Linux { get => linux; set => linux = value; }

        public bool isCombinationUniqe(string name, int id)
        {
            foreach (Game game in gamesList)
            {
                if (game.id == id || game.name == name)
                {
                    return false;
                }
            }
            return true;
        }

        public bool insert(Game game)
        {
            if (!isCombinationUniqe(game.name, game.id)) {return false;}
            DBservices db = new DBservices();
            int newGameId = db.InsertAGame(game);
            if (newGameId > 0)
            {
                if (game.Tags != null && game.Tags.Count > 0)
                {
                    foreach (string tag in game.Tags)
                    {
                        db.AddTagToGame(newGameId, tag);
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<Game> read() { 
            DBservices db = new DBservices();
            List<Game> gamesList = db.ShowAllGames();
            return gamesList;
        }

        public int delete(int id)
        {
            DBservices db = new DBservices();
            return db.DeleteGameById(id);
        }

        public List<Game> GetByName(string name)
        {
            DBservices db = new DBservices();
            List<Game> gamesList = db.ShowGameByName(name);
            return gamesList;
        }

        public static int UpdateGame(int id, Game game)
        {
            DBservices db = new DBservices();
            int result = db.UpdateAGameById(id, game);
            if (game.Tags != null && game.Tags.Count > 0)
            {
                foreach (string tag in game.Tags)
                {
                    db.AddTagToGame(id, tag);
                }
            }
            return result;
        }

        public List<Game> GetRecommendedGames(int id)
        {
            DBservices db = new DBservices();

            List<Game> games = db.ShowAllGames();
            List<Game> gamesofauser = db.ShowAllUserGames(id);
            List<Game> gamestoreturn = new List<Game>();
            List<string> tags = db.GetAllTagsExsistsForUser(id);
            List<int> ids = new List<int>();
            List<int> idstoreturn = new List<int>();

            foreach (Game game in gamesofauser)
            {
                ids.Add(game.id);
            }

            foreach (Game g in games)
            {

                if(ids.Contains(g.id) || idstoreturn.Contains(g.id))
                {
                    continue;
                }
                else 
                {
                    for (int i = 0; i < tags.Count; i++)
                    {
                        if (g.Tags.Contains(tags[i]))
                        {
                            gamestoreturn.Add(g);
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

            }

            return gamestoreturn;
        }

        public List<Game> GameByTags(string tags)
        {
            DBservices db = new DBservices();
            List<string> tagsList = tags.Split(',').Select(t => t.Trim()).Where(t => t != "").ToList();
            List<Game> games = db.ShowAllGames();
            List<Game> gamestoreturn = new List<Game>();

            foreach (Game g in games)
            {
                g.Tags = db.ShowtagsforGame(g.Id);
                for (int i = 0; i < tagsList.Count; i++)
                {
                    if (g.Tags.Contains(tagsList[i]))
                    {
                        if (i == tagsList.Count - 1)
                        {
                            gamestoreturn.Add(g);
                        }
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return gamestoreturn;
        }

        public List<string> AllTags()
        {
            DBservices db = new DBservices();

            List<string> taglist = db.GetAllTagsExsists();

            return taglist;
        }

        public async Task<string> UploadGames(IFormFile file)
        {
            // 1. Check if the file is valid
            if (file == null || file.Length == 0)
            {
                return "ERROR:File is empty or missing.";
            }

            // 2. Read JSON content from file
            string jsonContent = "";

            using (var streamReader = new StreamReader(file.OpenReadStream()))
            {
                jsonContent = await streamReader.ReadToEndAsync();
            }

            // 3. JSON options
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            try
            {
                // 4. Convert JSON to List<Game>
                List<Game> gamesList = JsonSerializer.Deserialize<List<Game>>(jsonContent, options);

                DBservices db = new DBservices();

                foreach (Game g in gamesList)
                {
                    db.InsertAGame(g);
                }

                return $"Success! Converted file into a list of {gamesList.Count} games.";
            }
            catch (JsonException ex)
            {
                return $"ERROR:Failed to convert JSON. Error: {ex.Message}";
            }
        }
    }
}


