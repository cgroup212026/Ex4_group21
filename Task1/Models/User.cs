using System.Collections.Generic;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
namespace Task1.Models

{
    public class User
    {
        int id;
        string name;
        string email;
        string password;
        bool active = true;
        public static List<User> usersList = new List<User>();

        public User()
        {

        }

        public User(int id, string name, string email, string password, bool active)
        {
            Id = id;
            Name = name;
            Email = email;
            Password = password;
            Active = active;
        }

        public int Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public string Email { get => email; set => email = value; }
        public string Password { get => password; set => password = SetPassword(value); }
        public string SetPassword (string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        public bool Active { get => active; set => active = value; }

        public bool isCombinationUniqe(string email, int id)
        {
            foreach (User user in usersList)
            {
                if (user.id == id || user.email == email)  return false;
            }
            return true;
        }

        public int insert(User user)
        {
            if (!isCombinationUniqe(user.Email, user.Id)) { return 0; }
            DBservices db = new DBservices();
            int id = db.InsertAUser(user);
            return 1;
        }

        public List<User> read()
        {
            DBservices db = new DBservices();

            List<User> userslist = db.ShowAllUsers();

            return userslist;
        }
        public List<Game> GetUserGames(int id)
        {
            DBservices db = new DBservices();

            List<Game> userslist = db.ShowAllUserGames(id);

            return userslist;
        }
        public List<string> login(string email, string password)
        {
            DBservices db = new DBservices();

            string hashedPassword = SetPassword(password);

            List<string> value = db.LoginUser(email, hashedPassword);

            return value;
        }

        public int insertgametbl(int userid, int gameid)
        {
            DBservices db = new DBservices();

            int id = db.InsertToUserGameTable(userid, gameid);
            return id;
        }

        public string Updatebyid(int id, User user)
        {
            DBservices db = new DBservices();

            return db.UpdateAUserById(id, user);
        }


        public int DeleteUser(int id)
        {
            DBservices db = new DBservices();
            return db.DeleteUserById(id);
        }

        public int DeleteUserGames(int userId, int gameId)
        {
            DBservices db = new DBservices();
            return db.DeleteUserGame(userId, gameId);
        }
    }
}
