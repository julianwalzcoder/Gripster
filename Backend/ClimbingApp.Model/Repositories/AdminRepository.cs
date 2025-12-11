using System;
using ClimbingApp.Model.Entities;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;

namespace ClimbingApp.Model.Repositories;

public class AdminRepository : BaseRepository
{
    public AdminRepository(IConfiguration configuration) : base(configuration) { }
    
    public Admin GetAdminById(int id)
    {
        NpgsqlConnection dbConn = null;
        try
        {
            dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = "select * from \"Admin\" where \"ID\" = @id";
            cmd.Parameters.Add("@id", NpgsqlDbType.Integer).Value = id;
            
            var data = GetData(dbConn, cmd);
            if (data != null)
            {
                if (data.Read())
                {
                    return new Admin(Convert.ToInt32(data["ID"]))
                    {
                        GymID = Convert.ToInt32(data["GymID"]),
                        UserID = Convert.ToInt32(data["UserID"])
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
    
    public Admin? GetByUserId(int userId)
    {
        using var db = new NpgsqlConnection(ConnectionString);
        using var cmd = db.CreateCommand();
        cmd.CommandText = @"SELECT ""ID"", ""UserID"", ""GymID""
                            FROM ""Admin"" WHERE ""UserID"" = @uid";
        cmd.Parameters.AddWithValue("@uid", NpgsqlDbType.Integer, userId);
        using var r = GetData(db, cmd);
        if (r != null && r.Read())
        {
            return new Admin(r.GetInt32(0))
            {
                UserID = r.GetInt32(1),
                GymID = r.GetInt32(2)
            };
        }
        return null;
    }
    
    public List<Admin> GetAdmins()
    {
        NpgsqlConnection dbConn = null;
        var admins = new List<Admin>();
        try
        {
            dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = "select * from \"Admin\"";
            
            var data = GetData(dbConn, cmd);
            if (data != null)
            {
                while (data.Read())
                {
                    Admin a = new Admin(Convert.ToInt32(data["ID"]))
                    {
                        GymID = Convert.ToInt32(data["GymID"]),
                        UserID = Convert.ToInt32(data["UserID"])
                    };
                    admins.Add(a);
                }
            }
            return admins;
        }
        finally
        {
            dbConn?.Close();
        }
    }
    
    public bool InsertAdmin(Admin a)
    {
        NpgsqlConnection dbConn = null;
        try
        {
            dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = @"
insert into ""Admin""
(""GymID"", ""UserID"")
values
(@gymid, @userid)
";
            cmd.Parameters.AddWithValue("@gymid", NpgsqlDbType.Integer, a.GymID);
            cmd.Parameters.AddWithValue("@userid", NpgsqlDbType.Integer, a.UserID);
            
            bool result = InsertData(dbConn, cmd);
            return result;
        }
        finally
        {
            dbConn?.Close();
        }
    }
    
    public bool UpdateAdmin(Admin a)
    {
        var dbConn = new NpgsqlConnection(ConnectionString);
        var cmd = dbConn.CreateCommand();
        cmd.CommandText = @"
update ""Admin"" set
""GymID""=@gymid,
""UserID""=@userid
where
""ID"" = @id";
        cmd.Parameters.AddWithValue("@gymid", NpgsqlDbType.Integer, a.GymID);
        cmd.Parameters.AddWithValue("@userid", NpgsqlDbType.Integer, a.UserID);
        cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, a.Id);
        
        bool result = UpdateData(dbConn, cmd);
        return result;
    }
    
    public bool DeleteAdmin(int id)
    {
        var dbConn = new NpgsqlConnection(ConnectionString);
        var cmd = dbConn.CreateCommand();
        cmd.CommandText = @"
delete from ""Admin""
where ""ID"" = @id
";
        cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, id);
        
        bool result = DeleteData(dbConn, cmd);
        return result;
    }
}
