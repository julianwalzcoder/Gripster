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

        public User ValidateUser(string username, string password)
        {
            NpgsqlConnection dbConn = null;
            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();

                cmd.CommandText = @"SELECT u.""ID"", u.""Username"", u.""Mail"", u.""PasswordHash"",
                                           CASE WHEN a.""UserID"" IS NOT NULL THEN 'admin' ELSE 'user' END AS ""Role""
                                    FROM ""User"" u
                                    LEFT JOIN ""Admin"" a ON u.""ID"" = a.""UserID""
                                    WHERE u.""Username"" = @username
                                      AND u.""PasswordHash"" = crypt(@password, u.""PasswordHash"");";

                cmd.Parameters.AddWithValue("@username", NpgsqlDbType.Text, username);
                cmd.Parameters.AddWithValue("@password", NpgsqlDbType.Text, password);

                var data = GetData(dbConn, cmd);

                if (data != null && data.Read())
                {
                    return new User((int)data["ID"])
                    {
                        Username = data["Username"].ToString(),
                        Mail = data["Mail"].ToString(),
                        PasswordHash = data["PasswordHash"].ToString(),
                        Role = data["Role"].ToString()
                    };
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