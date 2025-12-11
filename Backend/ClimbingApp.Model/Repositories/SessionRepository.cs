using System;
using ClimbingApp.Model.Entities;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;

namespace ClimbingApp.Model.Repositories;

public class SessionRepository : BaseRepository
{
    public SessionRepository(IConfiguration configuration) : base(configuration) { }
    
    public Session GetSessionById(int id)
    {
        NpgsqlConnection dbConn = null;
        try
        {
            dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = "select * from \"UserSessionRoute\" where \"UserID\" = @id";
            cmd.Parameters.Add("@id", NpgsqlDbType.Integer).Value = id;
            
            var data = GetData(dbConn, cmd);
            if (data != null && data.Read())
            {
                var s = new Session
                {
                    ID = Convert.ToInt32(data["ID"]),
                    UserID = Convert.ToInt32(data["UserID"]),
                    RouteID = Convert.ToInt32(data["RouteID"]),
                    Status = data["Status"] == DBNull.Value ? null : data["Status"].ToString(),
                    LoggedAt = Convert.ToDateTime(data["LoggedAt"]),
                };
                return s;
            }
            return null;
        }
        finally
        {
            dbConn?.Close();
        }
    }
    
    public List<Session> GetSessions()
    {
        NpgsqlConnection dbConn = null;
        var sessions = new List<Session>();
        try
        {
            dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = "select * from \"UserSessionRoute\"";
            
            var data = GetData(dbConn, cmd);
            if (data != null)
            {
                while (data.Read())
                {
                    var s = new Session
                    {
                        ID = Convert.ToInt32(data["ID"]),
                        UserID = Convert.ToInt32(data["UserID"]),
                        RouteID = Convert.ToInt32(data["RouteID"]),
                        Status = data["Status"] == DBNull.Value ? null : data["Status"].ToString(),
                        LoggedAt = Convert.ToDateTime(data["LoggedAt"])
                    };
                    sessions.Add(s);
                }
            }
            return sessions;
        }
        finally
        {
            dbConn?.Close();
        }
    }
    
    //Is this needed?
    public bool InsertSession(Session s)
    {
        NpgsqlConnection dbConn = null;
        try
        {
            dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = @"
insert into ""UserSessionRoute""
(""UserID"", ""RouteID"", ""Status"", ""LoggedAt"")
values
(@userid, @routeid, @status, @loggedat)
";
            cmd.Parameters.AddWithValue("@userid", NpgsqlDbType.Integer, s.UserID);
            cmd.Parameters.AddWithValue("@routeid", NpgsqlDbType.Integer, s.RouteID);
            cmd.Parameters.AddWithValue("@status", NpgsqlDbType.Text, (object?)s.Status ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@loggedat", NpgsqlDbType.Timestamp, s.LoggedAt);

            bool result = InsertData(dbConn, cmd);
            return result;
        }
        finally
        {
            dbConn?.Close();
        }
    }
    
    public bool UpdateSession(Session s)
    {
        var dbConn = new NpgsqlConnection(ConnectionString);
        var cmd = dbConn.CreateCommand();
        cmd.CommandText = @"
update ""UserSessionRoute"" set
""UserID""=@userid,
""RouteID""=@routeid,
""Status""=@status,
""LoggedAt""=@loggedat
where
""ID"" = @id";
        cmd.Parameters.AddWithValue("@userid", NpgsqlDbType.Integer, s.UserID);
        cmd.Parameters.AddWithValue("@routeid", NpgsqlDbType.Integer, s.RouteID);
        cmd.Parameters.AddWithValue("@status", NpgsqlDbType.Text, (object?)s.Status ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@loggedat", NpgsqlDbType.Timestamp, s.LoggedAt);
        cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, s.ID);

        bool result = UpdateData(dbConn, cmd);
        return result;
    }
    
    public bool DeleteSession(int id)
    {
        var dbConn = new NpgsqlConnection(ConnectionString);
        var cmd = dbConn.CreateCommand();
        cmd.CommandText = @"
delete from ""UserSessionRoute""
where ""ID"" = @id
";
        cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, id);
        
        bool result = DeleteData(dbConn, cmd);
        return result;
    }

    public List<Session> GetSessionsByUser(int userId)
    {
        var list = new List<Session>();
        using var dbConn = new NpgsqlConnection(ConnectionString);
        var cmd = dbConn.CreateCommand();
        cmd.CommandText = @"SELECT ""ID"",""UserID"",""RouteID"",""Status"",""LoggedAt"" FROM ""UserSessionRoute"" WHERE ""UserID""=@uid ORDER BY ""LoggedAt"" DESC";
        cmd.Parameters.AddWithValue("@uid", NpgsqlDbType.Integer, userId);
        var r = GetData(dbConn, cmd);
        if (r != null)
        {
            while (r.Read())
            {
                var s = new Session
                {
                    ID = (int)r["ID"],
                    UserID = (int)r["UserID"],
                    RouteID = (int)r["RouteID"],
                    Status = r["Status"] == DBNull.Value ? null : r["Status"].ToString(),
                    LoggedAt = (DateTime)r["LoggedAt"]
                };
                list.Add(s);
            }
        }
        return list;
    }

    public int CreateSession(int userId, int routeId, string? status, DateTime loggedAt)
    {
        using var dbConn = new NpgsqlConnection(ConnectionString);
        var cmd = dbConn.CreateCommand();
        cmd.CommandText = @"
INSERT INTO ""UserSessionRoute"" (""UserID"",""RouteID"",""Status"",""LoggedAt"")
VALUES (@uid,@routeid,@status,@loggedat)
RETURNING ""ID""";
        cmd.Parameters.AddWithValue("@uid", NpgsqlDbType.Integer, userId);
        cmd.Parameters.AddWithValue("@routeid", NpgsqlDbType.Integer, routeId);
        cmd.Parameters.AddWithValue("@status", NpgsqlDbType.Text, (object?)status ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@loggedat", NpgsqlDbType.Timestamp, loggedAt);
        var reader = GetData(dbConn, cmd);
        if (reader != null && reader.Read()) return (int)reader["ID"];
        return 0;
    }
}
