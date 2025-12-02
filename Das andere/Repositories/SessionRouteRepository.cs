using System;
using ClimbingApp.Model.Entities;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;

namespace ClimbingApp.Model.Repositories;

public class SessionRouteRepository : BaseRepository
{
    public SessionRouteRepository(IConfiguration configuration) : base(configuration) { }
    
    public SessionRoute GetSessionRouteById(int sessionId, int routeId)
    {
        NpgsqlConnection dbConn = null;
        try
        {
            dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = "select * from \"SessionRoute\" where \"SessionID\" = @sessionid AND \"RouteID\" = @routeid";
            cmd.Parameters.Add("@sessionid", NpgsqlDbType.Integer).Value = sessionId;
            cmd.Parameters.Add("@routeid", NpgsqlDbType.Integer).Value = routeId;
            
            var data = GetData(dbConn, cmd);
            if (data != null)
            {
                if (data.Read())
                {
                    return new SessionRoute
                    {
                        SessionID = Convert.ToInt32(data["SessionID"]),
                        RouteID = Convert.ToInt32(data["RouteID"]),
                        Tries = Convert.ToInt32(data["Tries"]),
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
    
    public List<SessionRoute> GetSessionRoutes()
    {
        NpgsqlConnection dbConn = null;
        var sessionRoutes = new List<SessionRoute>();
        try
        {
            dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = "select * from \"SessionRoute\"";
            
            var data = GetData(dbConn, cmd);
            if (data != null)
            {
                while (data.Read())
                {
                    SessionRoute sr = new SessionRoute
                    {
                        SessionID = Convert.ToInt32(data["SessionID"]),
                        RouteID = Convert.ToInt32(data["RouteID"]),
                        Tries = Convert.ToInt32(data["Tries"]),
                    };
                    sessionRoutes.Add(sr);
                }
            }
            return sessionRoutes;
        }
        finally
        {
            dbConn?.Close();
        }
    }
    
    public bool InsertSessionRoute(SessionRoute sr)
    {
        NpgsqlConnection dbConn = null;
        try
        {
            dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = @"
INSERT INTO ""SessionRoute"" (""SessionID"", ""RouteID"", ""Tries"")
VALUES (@sessionid, @routeid, @tries)";
            cmd.Parameters.AddWithValue("@sessionid", NpgsqlDbType.Integer, sr.SessionID);
            cmd.Parameters.AddWithValue("@routeid", NpgsqlDbType.Integer, sr.RouteID);
            cmd.Parameters.AddWithValue("@tries", NpgsqlDbType.Integer, sr.Tries);
            return InsertData(dbConn, cmd);
        }
        finally { dbConn?.Close(); }
    }
    
    public bool UpdateSessionRoute(SessionRoute sr)
    {
        using var dbConn = new NpgsqlConnection(ConnectionString);
        var cmd = dbConn.CreateCommand();
        cmd.CommandText = @"
UPDATE ""SessionRoute""
SET ""Tries"" = @tries
WHERE ""SessionID"" = @sessionid AND ""RouteID"" = @routeid";
        cmd.Parameters.AddWithValue("@tries", NpgsqlDbType.Integer, sr.Tries);
        cmd.Parameters.AddWithValue("@sessionid", NpgsqlDbType.Integer, sr.SessionID);
        cmd.Parameters.AddWithValue("@routeid", NpgsqlDbType.Integer, sr.RouteID);
        return UpdateData(dbConn, cmd);
    }
    
    public bool DeleteSessionRoute(int sessionId, int routeId)
    {
        using var dbConn = new NpgsqlConnection(ConnectionString);
        var cmd = dbConn.CreateCommand();
        cmd.CommandText = @"DELETE FROM ""SessionRoute"" WHERE ""SessionID""=@sessionid AND ""RouteID""=@routeid";
        cmd.Parameters.AddWithValue("@sessionid", NpgsqlDbType.Integer, sessionId);
        cmd.Parameters.AddWithValue("@routeid", NpgsqlDbType.Integer, routeId);
        return DeleteData(dbConn, cmd);
    }
}
