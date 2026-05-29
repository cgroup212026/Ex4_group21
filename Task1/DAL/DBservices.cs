using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using Task1.Models;

/// <summary>
/// DBServices is a class created by me to provides some DataBase Services
/// </summary>
public class DBservices
{

    public DBservices()
    {

    }

    //--------------------------------------------------------------------------------------------------
    // This method creates a connection to the database according to the connectionString name in the web.config 
    //--------------------------------------------------------------------------------------------------
    public SqlConnection connect(String conString)
    {

        // read the connection string from the configuration file
        IConfigurationRoot configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json").Build();
        string cStr = configuration.GetConnectionString(conString);
        SqlConnection con = new SqlConnection(cStr);
        con.Open();
        return con;
    }


    //--------------------------------------------------------------------------------------------------
    // This method inserts a game to the games table 
    //--------------------------------------------------------------------------------------------------
    public int InsertAGame(Game game)
    {

        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        Dictionary<string, object> paramDic = new Dictionary<string, object>();
        paramDic.Add("@Name", game.Name);
        paramDic.Add("@SteamURL", game.SteamURL);
        paramDic.Add("@img", game.Img);
        paramDic.Add("@releaseDate", game.ReleaseDate);
        paramDic.Add("@reviewSummary", game.ReviewSummary);
        paramDic.Add("@price", game.Price);
        paramDic.Add("@windows", game.Windows);
        paramDic.Add("@mac", game.Mac);
        paramDic.Add("@linux", game.Linux);


        cmd = CreateCommandWithStoredProcedureGeneral("TW_InsertGame", con, paramDic);          // create the command

        try
        {
            int newlyGeneratedId = Convert.ToInt32(cmd.ExecuteScalar());

            foreach (string tag in game.Tags)
            {
                AddTagToGame(newlyGeneratedId, tag);
            }
            return newlyGeneratedId;
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();
            }
        }

    }



    //--------------------------------------------------------------------------------------------------
    // This method inserts tags to a game
    //--------------------------------------------------------------------------------------------------
    public int AddTagToGame(int id, string tag)
    {

        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (SqlException ex)
        {
            if (ex.Number == 2627 || ex.Number == 2601)
            {
                // 2627 = primary key violation
                // 2601 = unique index violation
                return -1; // duplicate
            }

            return -2; // other SQL error
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        Dictionary<string, object> paramDic = new Dictionary<string, object>();
        paramDic.Add("@Id", id);
        paramDic.Add("@Tag", tag);



        cmd = CreateCommandWithStoredProcedureGeneral("TW_AddTagToGame", con, paramDic);          // create the command

        try
        {
            int newlyGeneratedId = Convert.ToInt32(cmd.ExecuteScalar());

            return newlyGeneratedId;
        }
        catch (SqlException ex)
        {
            if (ex.Number == 2627 || ex.Number == 2601)
            {
                // 2627 = primary key violation
                // 2601 = unique index violation
                return -1; // duplicate
            }

            return -2; // other SQL error
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();
            }
        }

    }

    //--------------------------------------------------------------------------------------------------
    // This method Update a game By Id to the games table 
    //--------------------------------------------------------------------------------------------------
    public int UpdateAGameById(int id, Game game)
    {

        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        Dictionary<string, object> paramDic = new Dictionary<string, object>();
        paramDic.Add("@Id", id);
        paramDic.Add("@Name", game.Name);
        paramDic.Add("@SteamURL", game.SteamURL);
        paramDic.Add("@img", game.Img);
        paramDic.Add("@releaseDate", game.ReleaseDate);
        paramDic.Add("@reviewSummary", game.ReviewSummary);
        paramDic.Add("@price", game.Price);
        paramDic.Add("@windows", game.Windows);
        paramDic.Add("@mac", game.Mac);
        paramDic.Add("@linux", game.Linux);


        cmd = CreateCommandWithStoredProcedureGeneral("TW_UpdateGameById", con, paramDic);          // create the command

        try
        {
            int numEffected = cmd.ExecuteNonQuery(); // execute the command
            return numEffected;
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();
            }
        }

    }

    //--------------------------------------------------------------------------------------------------
    // This method Shows all games from the games table 
    //--------------------------------------------------------------------------------------------------
    public List<Game> ShowAllGames()
    {

        SqlConnection con;
        SqlCommand cmd;
        List<Game> games = new List<Game>();


        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        cmd = CreateCommandWithStoredProcedureGeneral("TW_GetGames", con, null);          // create the command                                                                                         //    SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
        
        SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

        try
        {

            while (dataReader.Read())
            {
                Game g = new Game();
                g.Id = Convert.ToInt32(dataReader["Id"]);
                g.Name = dataReader["Name"].ToString();
                g.SteamURL = dataReader["SteamURL"].ToString();
                g.Img = dataReader["Img"].ToString();
                g.ReleaseDate = dataReader["ReleaseDate"].ToString();
                g.ReviewSummary = dataReader["ReviewSummary"].ToString();
                g.Price = Convert.ToDouble(dataReader["Price"]);
                g.Tags = ShowtagsforGame(g.Id);
                g.Windows = Convert.ToBoolean(dataReader["Windows"]);
                g.Mac = Convert.ToBoolean(dataReader["Mac"]);
                g.Linux = Convert.ToBoolean(dataReader["Linux"]);
                games.Add(g);
            }

            return games;


        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }



        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();
            }
        }


    }


    //--------------------------------------------------------------------------------------------------
    // This method Shows all tags from the a game table for a specific game 
    //--------------------------------------------------------------------------------------------------
    public List<string> ShowtagsforGame(int id)
    {

        SqlConnection con;
        SqlCommand cmd;
        List<string> tags = new List<string>();


        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }


        Dictionary<string, object> paramDic = new Dictionary<string, object>();
        paramDic.Add("@Id", id);

        cmd = CreateCommandWithStoredProcedureGeneral("TW_GetTags", con, paramDic);          // create the command                                                                                         //    SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

        SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

        try
        {

            while (dataReader.Read())
            {
                tags.Add(dataReader["TagName"].ToString());
            }

            return tags;


        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }



        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();
            }
        }


    }




    //--------------------------------------------------------------------------------------------------
    // This method Show a game By Id from the games table 
    //--------------------------------------------------------------------------------------------------
    public List<Game> ShowGameByName(string name)
    {

        SqlConnection con;
        SqlCommand cmd;
        List<Game> games = new List<Game>();

        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        Dictionary<string, object> paramDic = new Dictionary<string, object>();
        paramDic.Add("@name", name);

        cmd = CreateCommandWithStoredProcedureGeneral("TW_GetGameByName", con, paramDic);          // create the command

        SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

        try
        {
            Game g = null;
            while (dataReader.Read())
            {
                g = new Game();
                g.Id = Convert.ToInt32(dataReader["Id"]);
                g.Name = dataReader["Name"].ToString();
                g.SteamURL = dataReader["SteamURL"].ToString();
                g.Img = dataReader["Img"].ToString();
                g.ReleaseDate = dataReader["ReleaseDate"].ToString();
                g.ReviewSummary = dataReader["ReviewSummary"].ToString();
                g.Price = Convert.ToDouble(dataReader["Price"]);
                g.Tags = ShowtagsforGame(g.Id);
                g.Windows = Convert.ToBoolean(dataReader["Windows"]);
                g.Mac = Convert.ToBoolean(dataReader["Mac"]);
                g.Linux = Convert.ToBoolean(dataReader["Linux"]);
                games.Add(g);
            }
            return games;
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();
            }
        }

    }

    //--------------------------------------------------------------------------------------------------
    // This method Delete a game By Id from the games table 
    //--------------------------------------------------------------------------------------------------
    public int DeleteGameById(int id)
    {

        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        Dictionary<string, object> paramDic = new Dictionary<string, object>();
        paramDic.Add("@Id", id);

        cmd = CreateCommandWithStoredProcedureGeneral("TW_DeleteGameById", con, paramDic);          // create the command

        try
        {
            int numEffected = cmd.ExecuteNonQuery(); // execute the command
            return numEffected;
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();
            }
        }

    }


    //--------------------------------------------------------------------------------------------------
    // This method Shows all tags from the a game table 
    //--------------------------------------------------------------------------------------------------
    public List<string> GetAllTagsExsists()
    {

        SqlConnection con;
        SqlCommand cmd;
        List<string> tags = new List<string>();


        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }



        cmd = CreateCommandWithStoredProcedureGeneral("TW_GetAllTagsFromAllGames", con, null);          // create the command                                                                                         //    SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

        SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

        try
        {

            while (dataReader.Read())
            {
                tags.Add(dataReader["TagName"].ToString());
            }

            return tags;


        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }



        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();
            }
        }
    }


    //--------------------------------------------------------------------------------------------------
    // This method Shows all tags for a single user
    //--------------------------------------------------------------------------------------------------
    public List<string> GetAllTagsExsistsForUser(int id)
    {

        SqlConnection con;
        SqlCommand cmd;
        List<string> tags = new List<string>();


        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }


        Dictionary<string, object> paramDic = new Dictionary<string, object>();
        paramDic.Add("@Id", id);

        cmd = CreateCommandWithStoredProcedureGeneral("TW_GetAllTagsForSingleUser", con, paramDic);          // create the command                                                                                         //    SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

        SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

        try
        {

            while (dataReader.Read())
            {
                tags.Add(dataReader["TagName"].ToString());
            }
            return tags;
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }



        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();
            }
        }
    }


    ////--------------------------------------------------------------------------------------------------
    //// 
    ////
    ////
    ////
    ////                    
    ////                 THIS IS THE USERS DBservices
    ////
    ////
    ////
    ////
    ////
    ////--------------------------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------------------------
    // This method show all users from the users table 
    //--------------------------------------------------------------------------------------------------
    public List<User> ShowAllUsers()
    {

        SqlConnection con;
        SqlCommand cmd;
        List<User> users = new List<User>();

        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        cmd = CreateCommandWithStoredProcedureGeneral("TW_ShowAllUsers", con, null);          // create the command

        SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

        try
        {

            while (dataReader.Read())
            {
                User u = new User();
                u.Id = Convert.ToInt32(dataReader["Id"]);
                u.Name = dataReader["Name"].ToString();
                u.Email = dataReader["Email"].ToString();
                u.Password = dataReader["Password"].ToString();
                u.Active = Convert.ToBoolean(dataReader["Active"]);
                users.Add(u);
            }
            return users;
        }

        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();
            }
        }

    }

    //--------------------------------------------------------------------------------------------------
    // This method inserts a user to the users table 
    //--------------------------------------------------------------------------------------------------
    public int InsertAUser(User user)
    {

        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        Dictionary<string, object> paramDic = new Dictionary<string, object>();
        paramDic.Add("@Name", user.Name);
        paramDic.Add("@Email", user.Email);
        paramDic.Add("@Password", user.Password);
        paramDic.Add("@Active", user.Active);


        cmd = CreateCommandWithStoredProcedureGeneral("TW_InsertUser", con, paramDic);          // create the command

        try
        {
            int newlyGeneratedId = Convert.ToInt32(cmd.ExecuteScalar());

            return newlyGeneratedId;
        }
        catch (Exception ex)
        {
            // write to log
            return 0;
        }

        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();
            }
        }

    }


    //--------------------------------------------------------------------------------------------------
    // This method Update a user By Id to the users table 
    //--------------------------------------------------------------------------------------------------
    public string UpdateAUserById(int id, User user)
    {

        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        Dictionary<string, object> paramDic = new Dictionary<string, object>();
        paramDic.Add("@Id", id);
        paramDic.Add("@Name", user.Name);
        paramDic.Add("@Password", user.Password);



        cmd = CreateCommandWithStoredProcedureGeneral("TW_UpdateUserById", con, paramDic);          // create the command

        try
        {
            int numEffected = cmd.ExecuteNonQuery(); // execute the command
            return user.Name;
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();
            }
        }

    }

    //--------------------------------------------------------------------------------------------------
    // This method Delete a User By Id from the Users table 
    //--------------------------------------------------------------------------------------------------
    public int DeleteUserById(int id)
    {

        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        Dictionary<string, object> paramDic = new Dictionary<string, object>();
        paramDic.Add("@Id", id);

        cmd = CreateCommandWithStoredProcedureGeneral("TW_DeleteUserById", con, paramDic);          // create the command

        try
        {
            int numEffected = cmd.ExecuteNonQuery(); // execute the command
            return numEffected;
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();
            }
        }

    }



    //--------------------------------------------------------------------------------------------------
    // This method Loggs in a User By email and password
    //--------------------------------------------------------------------------------------------------
    public List<string> LoginUser(string email, string password)
    {

        SqlConnection con;
        SqlCommand cmd;
        List<string> strings = new List<string>();

        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        Dictionary<string, object> paramDic = new Dictionary<string, object>();
        paramDic.Add("@email", email);
        paramDic.Add("@password", password);

        cmd = CreateCommandWithStoredProcedureGeneral("TW_LoginUser", con, paramDic);          // create the command

        SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

        try
        {
            while (dataReader.Read())
            {
                string Namereturned = dataReader["Name"].ToString();
                string Idreturned = dataReader["Id"].ToString();
                strings.Add(Namereturned);
                strings.Add(Idreturned);
            }
            return strings;
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();
            }
        }

    }


    //--------------------------------------------------------------------------------------------------
    // This method show all games of a user from the UsersGames table 
    //--------------------------------------------------------------------------------------------------
    public List<Game> ShowAllUserGames(int id)
    {

        SqlConnection con;
        SqlCommand cmd;
        List<Game> games = new List<Game>();

        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        Dictionary<string, object> paramDic = new Dictionary<string, object>();
        paramDic.Add("@id", id);

        cmd = CreateCommandWithStoredProcedureGeneral("TW_ShowAllGamesOfAUser", con, paramDic);          // create the command

        SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

        try
        {

            while (dataReader.Read())
            {
                Game g = new Game();
                g.Id = Convert.ToInt32(dataReader["Id"]);
                g.Name = dataReader["Name"].ToString();
                g.SteamURL = dataReader["SteamURL"].ToString();
                g.Img = dataReader["Img"].ToString();
                g.ReleaseDate = dataReader["ReleaseDate"].ToString();
                g.ReviewSummary = dataReader["ReviewSummary"].ToString();
                g.Price = Convert.ToDouble(dataReader["Price"]);
                g.Tags = ShowtagsforGame(g.Id);
                g.Windows = Convert.ToBoolean(dataReader["Windows"]);
                g.Mac = Convert.ToBoolean(dataReader["Mac"]);
                g.Linux = Convert.ToBoolean(dataReader["Linux"]);
                games.Add(g);
            }

            return games;

        }

        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();
            }
        }

    }



    //--------------------------------------------------------------------------------------------------
    // This method inserts a user to the users table 
    //--------------------------------------------------------------------------------------------------
    public int InsertToUserGameTable(int userid, int gameid)
    {

        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        Dictionary<string, object> paramDic = new Dictionary<string, object>();
        paramDic.Add("@Userid", userid);
        paramDic.Add("@Gameid", gameid);


        cmd = CreateCommandWithStoredProcedureGeneral("TW_AddToUserGameTable", con, paramDic);          // create the command

        try
        {
            int numEffected = cmd.ExecuteNonQuery(); // execute the command
            return numEffected;
        }
        catch (Exception ex)
        {
            // write to log
            return 0;
        }

        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();
            }
        }

    }


    //--------------------------------------------------------------------------------------------------
    // This method Delete a User and a game By Ids from the UsersGames table 
    //--------------------------------------------------------------------------------------------------
    public int DeleteUserGame(int userid, int gameid)
    {

        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        Dictionary<string, object> paramDic = new Dictionary<string, object>();
        paramDic.Add("@UserId", userid);
        paramDic.Add("@GameId", gameid);

        cmd = CreateCommandWithStoredProcedureGeneral("TW_DeleteFromUserGameTable", con, paramDic);          // create the command

        try
        {
            int numEffected = cmd.ExecuteNonQuery(); // execute the command
            return numEffected;
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();
            }
        }

    }


    //---------------------------------------------------------------------------------
    // Create the SqlCommand
    //---------------------------------------------------------------------------------
    private SqlCommand CreateCommandWithStoredProcedureGeneral(String spName, SqlConnection con, Dictionary<string, object> paramDic)
    {

        SqlCommand cmd = new SqlCommand(); // create the command object

        cmd.Connection = con;              // assign the connection to the command object

        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 

        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds

        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        if (paramDic != null)
            foreach (KeyValuePair<string, object> param in paramDic)
            {
                cmd.Parameters.AddWithValue(param.Key, param.Value);

            }


        return cmd;
    }

    // TODO - ADD ReadFlights METHOD


    // TODO - ADD ReadFlightsFrom METHOD


}
