using Npgsql; using NpgsqlTypes;
using Microsoft.Extensions.Configuration;
namespace ClimbingApp.Model.Repositories;

public class UserSessionRouteRepository : BaseRepository
{
    public UserSessionRouteRepository(IConfiguration configuration) : base(configuration)
    {
    }
    public bool Insert(int userId, int routeId, string status, DateTimeOffset? loggedAt = null)
    {
        using var db = new NpgsqlConnection(ConnectionString);
        var cmd = db.CreateCommand();
        cmd.CommandText = @"INSERT INTO ""UserSessionRoute"" (""UserID"",""RouteID"",""Status"",""LoggedAt"")
                            VALUES (@uid,@rid,@status,@loggedAt)";
        cmd.Parameters.AddWithValue("@uid", NpgsqlDbType.Integer, userId);
        cmd.Parameters.AddWithValue("@rid", NpgsqlDbType.Integer, routeId);
        cmd.Parameters.AddWithValue("@status", NpgsqlDbType.Varchar, status);
        cmd.Parameters.AddWithValue("@loggedAt", NpgsqlDbType.TimestampTz, loggedAt ?? DateTimeOffset.UtcNow);
        return InsertData(db, cmd);
    }
}