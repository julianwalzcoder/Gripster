using System;
using ClimbingApp.Model.Entities;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;

namespace ClimbingApp.Model.Repositories;

public class ClimbRepository : BaseRepository
{
    public ClimbRepository(IConfiguration configuration) : base(configuration) { }
    
    public Climb GetRouteById(int id)
    {
        NpgsqlConnection dbConn = null;
        try
        {
            dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = "select * from \"Route\" where \"ID\" = @id";
            cmd.Parameters.Add("@id", NpgsqlDbType.Integer).Value = id;
            
            var data = GetData(dbConn, cmd);
            if (data != null)
            {
                if (data.Read())
                {
                    return new Climb(Convert.ToInt32(data["ID"]))
                    {
                        GymID = Convert.ToInt32(data["GymID"]),
                        GradeID = Convert.ToInt32(data["GradeID"]),
                        SetDate = Convert.ToDateTime(data["SetDate"]),
                        RemoveDate = data["RemoveDate"] == DBNull.Value ? null : Convert.ToDateTime(data["RemoveDate"]),
                        AdminID = Convert.ToInt32(data["AdminID"])
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
    
    public List<Climb> GetRoutes()
    {
        NpgsqlConnection dbConn = null;
        var routes = new List<Climb>();
        try
        {
            dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = "select * from \"Route\"";
            
            var data = GetData(dbConn, cmd);
            if (data != null)
            {
                while (data.Read())
                {
                    Climb r = new Climb(Convert.ToInt32(data["ID"]))
                    {
                        GymID = Convert.ToInt32(data["GymID"]),
                        GradeID = Convert.ToInt32(data["GradeID"]),
                        SetDate = Convert.ToDateTime(data["SetDate"]),
                        RemoveDate = data["RemoveDate"] == DBNull.Value ? null : Convert.ToDateTime(data["RemoveDate"]),
                        AdminID = Convert.ToInt32(data["AdminID"])
                    };
                    routes.Add(r);
                }
            }
            return routes;
        }
        finally
        {
            dbConn?.Close();
        }
    }

    public bool InsertRoute(Climb r)
    {
        NpgsqlConnection dbConn = null;
        try
        {
            dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = @"
insert into ""Route""
(""GymID"", ""GradeID"", ""SetDate"", ""RemoveDate"", ""AdminID"")
values
(@gymid, @gradeid, @setdate, @removedate, @adminid)
";
            cmd.Parameters.AddWithValue("@gymid", NpgsqlDbType.Integer, r.GymID);
            cmd.Parameters.AddWithValue("@gradeid", NpgsqlDbType.Integer, r.GradeID);
            cmd.Parameters.AddWithValue("@setdate", NpgsqlDbType.Date, r.SetDate);
            cmd.Parameters.AddWithValue("@removedate", r.RemoveDate.HasValue ? NpgsqlDbType.Date : NpgsqlDbType.Unknown, r.RemoveDate.HasValue ? r.RemoveDate.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@adminid", NpgsqlDbType.Integer, r.AdminID);
            
            bool result = InsertData(dbConn, cmd);
            return result;
        }
        finally
        {
            dbConn?.Close();
        }
    }
    
    public bool UpdateRoute(Climb r)
    {
        var dbConn = new NpgsqlConnection(ConnectionString);
        var cmd = dbConn.CreateCommand();
        cmd.CommandText = @"
update ""Route"" set
""GymID""=@gymid,
""GradeID""=@gradeid,
""SetDate""=@setdate,
""RemoveDate""=@removedate,
""AdminID""=@adminid
where
""ID"" = @id";
        cmd.Parameters.AddWithValue("@gymid", NpgsqlDbType.Integer, r.GymID);
        cmd.Parameters.AddWithValue("@gradeid", NpgsqlDbType.Integer, r.GradeID);
        cmd.Parameters.AddWithValue("@setdate", NpgsqlDbType.Date, r.SetDate);
        cmd.Parameters.AddWithValue("@removedate", r.RemoveDate.HasValue ? NpgsqlDbType.Date : NpgsqlDbType.Unknown, r.RemoveDate.HasValue ? r.RemoveDate.Value : DBNull.Value);
        cmd.Parameters.AddWithValue("@adminid", NpgsqlDbType.Integer, r.AdminID);
        cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, r.Id);
        
        bool result = UpdateData(dbConn, cmd);
        return result;
    }
    
    public bool DeleteRoute(int id)
    {
        var dbConn = new NpgsqlConnection(ConnectionString);
        var cmd = dbConn.CreateCommand();
        cmd.CommandText = @"
delete from ""Route""
where ""ID"" = @id
";
        cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, id);
        
        bool result = DeleteData(dbConn, cmd);
        return result;
    }

    public decimal? GetAverageRatingForRoute(int routeId)
    {
        using var dbConn = new NpgsqlConnection(ConnectionString);
        var cmd = dbConn.CreateCommand();
        cmd.CommandText = @"
SELECT AVG(""Rating"")
FROM ""UserRoute""
WHERE ""RouteID"" = @routeid AND ""Rating"" BETWEEN 1 AND 5";
        cmd.Parameters.AddWithValue("@routeid", NpgsqlDbType.Integer, routeId);

        var reader = GetData(dbConn, cmd);
        if (reader != null && reader.Read() && !reader.IsDBNull(0))
        {
            var avg = reader.GetDouble(0);
            return Convert.ToDecimal(avg);
        }
        return null;
    }
}
