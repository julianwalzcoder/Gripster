using ClimbingApp.Model.Entities;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using System;

namespace ClimbingApp.Model.Repositories
{
    public class AuthRepository : BaseRepository
    {
        public AuthRepository(IConfiguration configuration) : base(configuration) { }

        public virtual User ValidateUser(string username, string password)
        {
            NpgsqlConnection dbConn = null;
            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();

                cmd.CommandText = @"SELECT * FROM ""User"" WHERE ""Username"" = @username AND ""PasswordHash"" = crypt(@password, ""PasswordHash"");";

                cmd.Parameters.AddWithValue("@username", NpgsqlDbType.Text, username);
                cmd.Parameters.AddWithValue("@password", NpgsqlDbType.Text, password);

                var data = GetData(dbConn, cmd);

                if (data != null && data.Read())
                {
                    var user = new User((int)data["ID"])
                    {
                        Id = (int)data["ID"],
                        Username = data["Username"].ToString(),
                        Mail = data["Mail"].ToString(),
                        PasswordHash = data["PasswordHash"].ToString(),
                        Role = data["Role"].ToString()
                    };

                    // Close the first reader before executing another command
                    data.Close();

                    // If user is admin, fetch the Admin.ID
                    if (user.Role == "admin")
                    {
                        var adminCmd = dbConn.CreateCommand();
                        adminCmd.CommandText = @"SELECT ""ID"" FROM ""Admin"" WHERE ""UserID"" = @userId;";
                        adminCmd.Parameters.AddWithValue("@userId", NpgsqlDbType.Integer, user.Id);

                        using (var adminReader = adminCmd.ExecuteReader())
                        {
                            if (adminReader != null && adminReader.Read())
                            {
                                user.AdminId = (int)adminReader["ID"];
                            }
                        }
                    }

                    return user;
                }
                return null;
            }
            finally
            {
                dbConn?.Close();
            }
        }
    }
}