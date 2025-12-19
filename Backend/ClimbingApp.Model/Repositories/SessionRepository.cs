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
            cmd.CommandText = "select * from \"Session\" where \"ID\" = @id";
            cmd.Parameters.Add("@id", NpgsqlDbType.Integer).Value = id;
            
            var data = GetData(dbConn, cmd);
            if (data != null && data.Read())
            {
                var s = new Session
                {
                    ID = Convert.ToInt32(data["ID"]),
                    UserID = Convert.ToInt32(data["UserID"]),
                    CustomName = data["CustomName"] == DBNull.Value ? null : data["CustomName"].ToString(),
                    Date = Convert.ToDateTime(data["Date"]),
                    Feedback = data["Feedback"] == DBNull.Value ? null : data["Feedback"].ToString()
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
            cmd.CommandText = "select * from \"Session\"";
            
            var data = GetData(dbConn, cmd);
            if (data != null)
            {
                while (data.Read())
                {
                    var s = new Session
                    {
                        ID = Convert.ToInt32(data["ID"]),
                        UserID = Convert.ToInt32(data["UserID"]),
                        CustomName = data["CustomName"] == DBNull.Value ? null : data["CustomName"].ToString(),
                        Date = Convert.ToDateTime(data["Date"]),
                        Feedback = data["Feedback"] == DBNull.Value ? null : data["Feedback"].ToString()
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
    
    public bool InsertSession(Session s)
    {
        NpgsqlConnection dbConn = null;
        try
        {
            dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = @"
insert into ""Session""
(""UserID"", ""CustomName"", ""Date"", ""Feedback"")
values
(@userid, @customname, @date, @feedback)
";
            cmd.Parameters.AddWithValue("@userid", NpgsqlDbType.Integer, s.UserID);
            cmd.Parameters.AddWithValue("@customname", NpgsqlDbType.Text, s.CustomName);
            cmd.Parameters.AddWithValue("@date", NpgsqlDbType.Date, s.Date);
            cmd.Parameters.AddWithValue("@feedback", NpgsqlDbType.Text, s.Feedback);
            
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
update ""Session"" set
""UserID""=@userid,
""CustomName""=@customname,
""Date""=@date,
""Feedback""=@feedback
where
""ID"" = @id";
        cmd.Parameters.AddWithValue("@userid", NpgsqlDbType.Integer, s.UserID);
        cmd.Parameters.AddWithValue("@customname", NpgsqlDbType.Text, (object?)s.CustomName ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@date", NpgsqlDbType.Date, s.Date);
        cmd.Parameters.AddWithValue("@feedback", NpgsqlDbType.Text, (object?)s.Feedback ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, s.ID);

        bool result = UpdateData(dbConn, cmd);
        return result;
    }
    
    public bool DeleteSession(int id)
    {
        var dbConn = new NpgsqlConnection(ConnectionString);
        var cmd = dbConn.CreateCommand();
        cmd.CommandText = @"
delete from ""Session""
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
        cmd.CommandText = @"SELECT ""ID"",""UserID"",""CustomName"",""Date"",""Feedback"" FROM ""Session"" WHERE ""UserID""=@uid ORDER BY ""Date"" DESC";
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
                    CustomName = r["CustomName"] == DBNull.Value ? null : r["CustomName"].ToString(),
                    Date = (DateTime)r["Date"],
                    Feedback = r["Feedback"] == DBNull.Value ? null : r["Feedback"].ToString()
                };
                list.Add(s);
            }
        }
        return list;
    }

    public int CreateSession(int userId, DateTime date, string? name, string? feedback)
    {
        using var dbConn = new NpgsqlConnection(ConnectionString);
        var cmd = dbConn.CreateCommand();
        cmd.CommandText = @"
INSERT INTO ""Session"" (""UserID"",""CustomName"",""Date"",""Feedback"")
VALUES (@uid,@name,@date,@fb)
RETURNING ""ID""";
        cmd.Parameters.AddWithValue("@uid", NpgsqlDbType.Integer, userId);
        cmd.Parameters.AddWithValue("@name", NpgsqlDbType.Text, (object?)name ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@date", NpgsqlDbType.Date, date);
        cmd.Parameters.AddWithValue("@fb", NpgsqlDbType.Text, (object?)feedback ?? DBNull.Value);
        var reader = GetData(dbConn, cmd);
        if (reader != null && reader.Read()) return (int)reader["ID"];
        return 0;
    }
}
