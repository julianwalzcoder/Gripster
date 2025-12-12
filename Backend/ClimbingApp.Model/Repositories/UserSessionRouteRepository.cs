using Npgsql; using NpgsqlTypes;
using Microsoft.Extensions.Configuration;

namespace ClimbingApp.Model.Repositories;

public class UserSessionRouteRepository : BaseRepository
{
    public UserSessionRouteRepository(IConfiguration configuration) : base(configuration) { }

    public (string Status, DateTimeOffset LoggedAt)? GetLast(int userId, int routeId)
    {
        using var db = new NpgsqlConnection(ConnectionString);
        db.Open();
        using var cmd = db.CreateCommand();
        cmd.CommandText = @"SELECT ""Status"", ""LoggedAt""
                            FROM ""UserSessionRoute""
                            WHERE ""UserID""=@uid AND ""RouteID""=@rid
                            ORDER BY ""LoggedAt"" DESC
                            LIMIT 1";
        cmd.Parameters.AddWithValue("@uid", NpgsqlDbType.Integer, userId);
        cmd.Parameters.AddWithValue("@rid", NpgsqlDbType.Integer, routeId);
        using var r = cmd.ExecuteReader();
        if (r.Read())
        {
            var status = r.GetString(0);
            var loggedAt = r.GetFieldValue<DateTimeOffset>(1);
            return (status, loggedAt);
        }
        return null;
    }

    public bool Insert(int userId, int routeId, string status, DateTimeOffset? loggedAt = null)
    {
        using var db = new NpgsqlConnection(ConnectionString);
        using var cmd = db.CreateCommand();
        cmd.CommandText = @"INSERT INTO ""UserSessionRoute"" (""UserID"",""RouteID"",""Status"",""LoggedAt"")
                            VALUES (@uid,@rid,@status,@loggedAt)";
        cmd.Parameters.AddWithValue("@uid", NpgsqlDbType.Integer, userId);
        cmd.Parameters.AddWithValue("@rid", NpgsqlDbType.Integer, routeId);
        cmd.Parameters.AddWithValue("@status", NpgsqlDbType.Varchar, status);
        cmd.Parameters.AddWithValue("@loggedAt", NpgsqlDbType.TimestampTz, loggedAt ?? DateTimeOffset.UtcNow);
        return InsertData(db, cmd);
    }
}