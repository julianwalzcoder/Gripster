using System;
using ClimbingApp.Model.Entities;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;

namespace ClimbingApp.Model.Repositories;

public class GymRepository : BaseRepository
{
    public GymRepository(IConfiguration configuration) : base(configuration) { }
    
    public Gym GetGymById(int id)
    {
        NpgsqlConnection dbConn = null;
        try
        {
            dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = "select * from \"Gym\" where \"ID\" = @id";
            cmd.Parameters.Add("@id", NpgsqlDbType.Integer).Value = id;
            
            var data = GetData(dbConn, cmd);
            if (data != null)
            {
                if (data.Read())
                {
                    return new Gym(Convert.ToInt32(data["ID"]))
                    {
                        Name = data["Name"].ToString(),
                        Street = data["Street"].ToString(),
                        StreetNumber = Convert.ToInt32(data["StreetNumber"]),
                        Postcode = Convert.ToInt32(data["Postcode"]),
                        City = data["City"].ToString()
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
    
    public List<(int Id, string Name)> GetGyms()
    {
        var gyms = new List<(int, string)>();
        using var dbConn = new NpgsqlConnection(ConnectionString);
        var cmd = dbConn.CreateCommand();
        cmd.CommandText = @"SELECT ""ID"", ""Name"" FROM ""Gym"" ORDER BY ""Name""";
        var reader = GetData(dbConn, cmd);
        if (reader != null)
        {
            while (reader.Read())
            {
                var id = (int)reader["ID"];
                var name = reader["Name"]?.ToString() ?? "";
                gyms.Add((id, name));
            }
        }
        return gyms;
    }
    
    public bool InsertGym(Gym g)
    {
        NpgsqlConnection dbConn = null;
        try
        {
            dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = @"
insert into ""Gym""
(""Name"", ""Street"", ""StreetNumber"", ""Postcode"", ""City"")
values
(@name, @street, @streetnumber, @postcode, @city)
";
            cmd.Parameters.AddWithValue("@name", NpgsqlDbType.Text, g.Name);
            cmd.Parameters.AddWithValue("@street", NpgsqlDbType.Text, g.Street);
            cmd.Parameters.AddWithValue("@streetnumber", NpgsqlDbType.Integer, g.StreetNumber);
            cmd.Parameters.AddWithValue("@postcode", NpgsqlDbType.Integer, g.Postcode);
            cmd.Parameters.AddWithValue("@city", NpgsqlDbType.Text, g.City);
            
            bool result = InsertData(dbConn, cmd);
            return result;
        }
        finally
        {
            dbConn?.Close();
        }
    }
    
    public bool UpdateGym(Gym g)
    {
        var dbConn = new NpgsqlConnection(ConnectionString);
        var cmd = dbConn.CreateCommand();
        cmd.CommandText = @"
update ""Gym"" set
""Name""=@name,
""Street""=@street,
""StreetNumber""=@streetnumber,
""Postcode""=@postcode,
""City""=@city
where
""ID"" = @id";
        cmd.Parameters.AddWithValue("@name", NpgsqlDbType.Text, g.Name);
        cmd.Parameters.AddWithValue("@street", NpgsqlDbType.Text, g.Street);
        cmd.Parameters.AddWithValue("@streetnumber", NpgsqlDbType.Integer, g.StreetNumber);
        cmd.Parameters.AddWithValue("@postcode", NpgsqlDbType.Integer, g.Postcode);
        cmd.Parameters.AddWithValue("@city", NpgsqlDbType.Text, g.City);
        cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, g.Id);
        
        bool result = UpdateData(dbConn, cmd);
        return result;
    }
    
    public bool DeleteGym(int id)
    {
        var dbConn = new NpgsqlConnection(ConnectionString);
        var cmd = dbConn.CreateCommand();
        cmd.CommandText = @"
delete from ""Gym""
where ""ID"" = @id
";
        cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, id);
        
        bool result = DeleteData(dbConn, cmd);
        return result;
    }
}
