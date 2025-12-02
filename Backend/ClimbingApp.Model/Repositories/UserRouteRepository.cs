using System;
using ClimbingApp.Model.Entities;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;

namespace ClimbingApp.Model.Repositories;

public class UserRouteRepository : BaseRepository
{
    public UserRouteRepository(IConfiguration configuration) : base(configuration) { }
    
    public UserRoute GetUserRouteById(int userId, int routeId)
    {
        NpgsqlConnection dbConn = null;
        try
        {
            dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = "select * from \"UserRoute\" where \"UserID\" = @userid AND \"RouteID\" = @routeid";
            cmd.Parameters.Add("@userid", NpgsqlDbType.Integer).Value = userId;
            cmd.Parameters.Add("@routeid", NpgsqlDbType.Integer).Value = routeId;
            
            var data = GetData(dbConn, cmd);
            if (data != null && data.Read())
            {
                return new UserRoute
                {
                    UserID = Convert.ToInt32(data["UserID"]),
                    RouteID = Convert.ToInt32(data["RouteID"]),
                    Status = data["Status"].ToString(),
                    Rating = data["Rating"] is DBNull ? null : Convert.ToInt32(data["Rating"])
                };
            }
            return null;
        }
        finally
        {
            dbConn?.Close();
        }
    }
    
    public List<UserRoute> GetUserRoutes()
    {
        NpgsqlConnection dbConn = null;
        var userRoutes = new List<UserRoute>();
        try
        {
            dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = "select * from \"UserRoute\"";
            
            var data = GetData(dbConn, cmd);
            if (data != null)
            {
                while (data.Read())
                {
                    UserRoute ur = new UserRoute
                    {
                        UserID = Convert.ToInt32(data["UserID"]),
                        RouteID = Convert.ToInt32(data["RouteID"]),
                        Status = data["Status"].ToString(),
                        Rating = data["Rating"] is DBNull ? null : Convert.ToInt32(data["Rating"])
                    };
                    userRoutes.Add(ur);
                }
            }
            return userRoutes;
        }
        finally
        {
            dbConn?.Close();
        }
    }
    
    public List<UserRoute> GetUserRoutesByUserId(int userId)
    {
        NpgsqlConnection dbConn = null;
        var userRoutes = new List<UserRoute>();
        try
        {
            dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = "select * from \"UserRoute\" where \"UserID\" = @userid";
            cmd.Parameters.Add("@userid", NpgsqlDbType.Integer).Value = userId;
            
            var data = GetData(dbConn, cmd);
            if (data != null)
            {
                while (data.Read())
                {
                    UserRoute ur = new UserRoute
                    {
                        UserID = Convert.ToInt32(data["UserID"]),
                        RouteID = Convert.ToInt32(data["RouteID"]),
                        Status = data["Status"].ToString(),
                        Rating = data["Rating"] is DBNull ? null : Convert.ToInt32(data["Rating"])
                    };
                    userRoutes.Add(ur);
                }
            }
            return userRoutes;
        }
        finally
        {
            dbConn?.Close();
        }
    }
    
    public List<UserRoute> GetUserRoutesByRouteId(int routeId)
    {
        NpgsqlConnection dbConn = null;
        var userRoutes = new List<UserRoute>();
        try
        {
            dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = "select * from \"UserRoute\" where \"RouteID\" = @routeid";
            cmd.Parameters.Add("@routeid", NpgsqlDbType.Integer).Value = routeId;
            
            var data = GetData(dbConn, cmd);
            if (data != null)
            {
                while (data.Read())
                {
                    UserRoute ur = new UserRoute
                    {
                        UserID = Convert.ToInt32(data["UserID"]),
                        RouteID = Convert.ToInt32(data["RouteID"]),
                        Status = data["Status"].ToString(),
                        Rating = data["Rating"] is DBNull ? null : Convert.ToInt32(data["Rating"])
                    };
                    userRoutes.Add(ur);
                }
            }
            return userRoutes;
        }
        finally
        {
            dbConn?.Close();
        }
    }
    
    public bool InsertUserRoute(UserRoute ur)
    {
        NpgsqlConnection dbConn = null;
        try
        {
            dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = @"
insert into ""UserRoute""
(""UserID"", ""RouteID"", ""Status"", ""Rating"")
values
(@userid, @routeid, @status, @rating)
";
            cmd.Parameters.AddWithValue("@userid", NpgsqlDbType.Integer, ur.UserID);
            cmd.Parameters.AddWithValue("@routeid", NpgsqlDbType.Integer, ur.RouteID);
            cmd.Parameters.AddWithValue("@status", NpgsqlDbType.Varchar, ur.Status);
            cmd.Parameters.AddWithValue("@rating", NpgsqlDbType.Integer, (object?)ur.Rating ?? DBNull.Value);
            
            bool result = InsertData(dbConn, cmd);
            return result;
        }
        finally
        {
            dbConn?.Close();
        }
    }
    public bool InsertUserRouteByID(int userID, int routeID, string status)
    {
        NpgsqlConnection dbConn = null;
        try
        {
            dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = @"
INSERT INTO ""UserRoute"" (""UserID"", ""RouteID"", ""Status"")
VALUES (@userid, @routeid, @status)
ON CONFLICT (""UserID"", ""RouteID"") 
DO UPDATE SET ""Status"" = @status
";
            cmd.Parameters.AddWithValue("@userid", NpgsqlDbType.Integer, userID);
            cmd.Parameters.AddWithValue("@routeid", NpgsqlDbType.Integer, routeID);
            cmd.Parameters.AddWithValue("@status", NpgsqlDbType.Varchar, status);

            bool result = InsertData(dbConn, cmd);
            return result;
        }
        finally
        {
            dbConn?.Close();
        }
    }
    
    public bool UpdateUserRoute(UserRoute ur)
    {
        var dbConn = new NpgsqlConnection(ConnectionString);
        var cmd = dbConn.CreateCommand();
        cmd.CommandText = @"
update ""UserRoute"" set
""Status""=@status,
""Rating""=@rating
where
""UserID"" = @userid AND ""RouteID"" = @routeid";
        cmd.Parameters.AddWithValue("@status", NpgsqlDbType.Varchar, ur.Status);
        cmd.Parameters.AddWithValue("@rating", NpgsqlDbType.Integer, (object?)ur.Rating ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@userid", NpgsqlDbType.Integer, ur.UserID);
        cmd.Parameters.AddWithValue("@routeid", NpgsqlDbType.Integer, ur.RouteID);
        
        bool result = UpdateData(dbConn, cmd);
        return result;
    }
    
    public bool DeleteUserRoute(int userId, int routeId)
    {
        var dbConn = new NpgsqlConnection(ConnectionString);
        var cmd = dbConn.CreateCommand();
        cmd.CommandText = @"
delete from ""UserRoute""
where ""UserID"" = @userid AND ""RouteID"" = @routeid
";
        cmd.Parameters.AddWithValue("@userid", NpgsqlDbType.Integer, userId);
        cmd.Parameters.AddWithValue("@routeid", NpgsqlDbType.Integer, routeId);
        
        bool result = DeleteData(dbConn, cmd);
        return result;
    }

    public bool UpsertRating(int userId, int routeId, int? rating)
    {
        using var dbConn = new NpgsqlConnection(ConnectionString);
        var cmd = dbConn.CreateCommand();
        cmd.CommandText = @"
INSERT INTO ""UserRoute"" (""UserID"", ""RouteID"", ""Rating"")
VALUES (@userid, @routeid, @rating)
ON CONFLICT (""UserID"", ""RouteID"")
DO UPDATE SET ""Rating"" = EXCLUDED.""Rating""";
        cmd.Parameters.AddWithValue("@userid", NpgsqlDbType.Integer, userId);
        cmd.Parameters.AddWithValue("@routeid", NpgsqlDbType.Integer, routeId);
        cmd.Parameters.AddWithValue("@rating", rating is null ? (object)DBNull.Value : rating);
        return InsertData(dbConn, cmd);
    }
}
