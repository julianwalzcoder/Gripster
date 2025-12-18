using System;
using ClimbingApp.Model.Entities;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using System.Data;

namespace ClimbingApp.Model.Repositories;

public class UserRepository : BaseRepository
{
    public UserRepository(IConfiguration configuration) : base(configuration) { }

    public virtual User GetUserById(int id)
    {
        NpgsqlConnection dbConn = null;
        try
        {
            dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = "SELECT * FROM \"User\" WHERE \"ID\" = @id";
            cmd.Parameters.Add("@id", NpgsqlDbType.Integer).Value = id;

            var data = GetData(dbConn, cmd);
            if (data != null)
            {
                if (data.Read())
                {
                    return new User(Convert.ToInt32(data["ID"]))
                    {
                        Name = data["Name"] == DBNull.Value ? null : data["Name"].ToString(),
                        Username = data["Username"] == DBNull.Value ? null : data["Username"].ToString(),
                        Mail = data["Mail"] == DBNull.Value ? null : data["Mail"].ToString(),
                        PasswordHash = data["PasswordHash"] == DBNull.Value ? null : data["PasswordHash"].ToString(),
                        Street = data["Street"] == DBNull.Value ? null : data["Street"].ToString(),
                        StreetNumber = data["StreetNumber"] == DBNull.Value ? 0 : Convert.ToInt32(data["StreetNumber"]),
                        Postcode = data["Postcode"] == DBNull.Value ? 0 : Convert.ToInt32(data["Postcode"]),
                        City = data["City"] == DBNull.Value ? null : data["City"].ToString(),
                        Role = data["Role"] == DBNull.Value ? null : data["Role"].ToString()
                    };
                }
            }
            return null;
        }
        finally
        {
            dbConn?.Close();
        }
    }

    public virtual List<User> GetUsers()
    {
        NpgsqlConnection dbConn = null;
        var users = new List<User>();
        try
        {
            dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = "SELECT * FROM \"User\"";

            var data = GetData(dbConn, cmd);
            if (data != null)
            {
                while (data.Read())
                {
                    User u = new User(Convert.ToInt32(data["ID"]))
                    {
                        Name = data["Name"] == DBNull.Value ? null : data["Name"].ToString(),
                        Username = data["Username"] == DBNull.Value ? null : data["Username"].ToString(),
                        Mail = data["Mail"] == DBNull.Value ? null : data["Mail"].ToString(),
                        PasswordHash = data["PasswordHash"] == DBNull.Value ? null : data["PasswordHash"].ToString(),
                        Street = data["Street"] == DBNull.Value ? null : data["Street"].ToString(),
                        StreetNumber = data["StreetNumber"] == DBNull.Value ? 0 : Convert.ToInt32(data["StreetNumber"]),
                        Postcode = data["Postcode"] == DBNull.Value ? 0 : Convert.ToInt32(data["Postcode"]),
                        City = data["City"] == DBNull.Value ? null : data["City"].ToString(),
                        Role = data["Role"] == DBNull.Value ? null : data["Role"].ToString()
                    };
                    users.Add(u);
                }
            }
            return users;
        }
        finally
        {
            dbConn?.Close();
        }
    }

    public virtual bool InsertUser(User u)
    {
        NpgsqlConnection dbConn = null;
        try
        {
            dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO ""User"" 
                (""Name"", ""Username"", ""Mail"", ""PasswordHash"", ""Street"", ""StreetNumber"", ""Postcode"", ""City"", ""Role"")
                VALUES
                (@name, @username, @mail, @passwordHash, @street, @streetnumber, @postcode, @city, @role)";
            cmd.Parameters.AddWithValue("@name", NpgsqlDbType.Text, u.Name);
            cmd.Parameters.AddWithValue("@username", NpgsqlDbType.Text, u.Username);
            cmd.Parameters.AddWithValue("@mail", NpgsqlDbType.Text, u.Mail);
            cmd.Parameters.AddWithValue("@passwordHash", NpgsqlDbType.Text, u.PasswordHash);
            cmd.Parameters.AddWithValue("@street", NpgsqlDbType.Text, u.Street);
            cmd.Parameters.AddWithValue("@streetnumber", NpgsqlDbType.Integer, u.StreetNumber);
            cmd.Parameters.AddWithValue("@postcode", NpgsqlDbType.Integer, u.Postcode);
            cmd.Parameters.AddWithValue("@city", NpgsqlDbType.Text, u.City);
            cmd.Parameters.AddWithValue("@role", NpgsqlDbType.Text, u.Role ?? "user"); // default to user

            bool result = InsertData(dbConn, cmd);
            return result;
        }
        finally
        {
            dbConn?.Close();
        }
    }

    public virtual bool UpdateUser(User u)
    {
        var dbConn = new NpgsqlConnection(ConnectionString);
        var cmd = dbConn.CreateCommand();
        cmd.CommandText = @"
UPDATE ""User"" SET
""Name""=@name,
""Username""=@username,
""Mail""=@mail,
""PasswordHash""=@passwordHash,
""Street""=@street,
""StreetNumber""=@streetnumber,
""Postcode""=@postcode,
""City""=@city,
""Role""=@role
WHERE ""ID"" = @id
";
        cmd.Parameters.AddWithValue("@name", NpgsqlDbType.Text, u.Name);
        cmd.Parameters.AddWithValue("@username", NpgsqlDbType.Text, u.Username);
        cmd.Parameters.AddWithValue("@mail", NpgsqlDbType.Text, u.Mail);
        cmd.Parameters.AddWithValue("@passwordHash", NpgsqlDbType.Text, u.PasswordHash);
        cmd.Parameters.AddWithValue("@street", NpgsqlDbType.Text, u.Street);
        cmd.Parameters.AddWithValue("@streetnumber", NpgsqlDbType.Integer, u.StreetNumber);
        cmd.Parameters.AddWithValue("@postcode", NpgsqlDbType.Integer, u.Postcode);
        cmd.Parameters.AddWithValue("@city", NpgsqlDbType.Text, u.City);
        cmd.Parameters.AddWithValue("@role", NpgsqlDbType.Text, u.Role ?? "user");
        cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, u.Id);

        bool result = UpdateData(dbConn, cmd);
        return result;
    }

    public virtual bool DeleteUser(int id)
    {
        var dbConn = new NpgsqlConnection(ConnectionString);
        var cmd = dbConn.CreateCommand();
        cmd.CommandText = @"DELETE FROM ""User"" WHERE ""ID"" = @id";
        cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, id);

        bool result = DeleteData(dbConn, cmd);
        return result;
    }

    public virtual bool InsertUser(User u, string plainPassword)
    {
        Console.WriteLine("Inserting user with username: " + u.Username);
        NpgsqlConnection dbConn = null;
        try
        {
            dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = @"
            INSERT INTO ""User"" 
            (""Name"", ""Username"", ""Mail"", ""PasswordHash"", ""Street"", ""StreetNumber"", ""Postcode"", ""City"", ""Role"")
            VALUES
            (@name, @username, @mail, crypt(@password, gen_salt('bf')), @street, @streetnumber, @postcode, @city, 'user')";
            cmd.Parameters.AddWithValue("@name", NpgsqlDbType.Text, u.Name);
            cmd.Parameters.AddWithValue("@username", NpgsqlDbType.Text, u.Username);
            cmd.Parameters.AddWithValue("@mail", NpgsqlDbType.Text, u.Mail);
            cmd.Parameters.AddWithValue("@password", NpgsqlDbType.Text, plainPassword);
            cmd.Parameters.AddWithValue("@street", NpgsqlDbType.Text, u.Street);
            cmd.Parameters.AddWithValue("@streetnumber", NpgsqlDbType.Integer, u.StreetNumber);
            cmd.Parameters.AddWithValue("@postcode", NpgsqlDbType.Integer, u.Postcode);
            cmd.Parameters.AddWithValue("@city", NpgsqlDbType.Text, u.City);

            // Role is always 'user' in SQL, not from u.Role

            bool result = InsertData(dbConn, cmd);
            return result;
        }
        finally
        {
            dbConn?.Close();
        }
    }

}