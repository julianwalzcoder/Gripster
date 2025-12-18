using ClimbingApp.Model.Entities;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;

namespace ClimbingApp.Model.Repositories
{
    public class UserRouteGradeRepository : BaseRepository
    {
        public UserRouteGradeRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public List<UserRouteGrade> GetUserRouteGrades()
        {
            NpgsqlConnection dbConn = null;
            var userRouteGrades = new List<UserRouteGrade>();
            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();
                cmd.CommandText = "SELECT * FROM UserRouteGradeView";
                var data = GetData(dbConn, cmd);
                if (data != null)
                {
                    while (data.Read())
                    {
                        userRouteGrades.Add(new UserRouteGrade
                        {
                            UserID = data["userid"] == DBNull.Value ? (int?)null : Convert.ToInt32(data["userid"]),
                            RouteID = Convert.ToInt32(data["routeid"]),
                            GradeFbleau = data["gradefbleau"].ToString(),
                            Status = data["status"].ToString(),
                            GymID = Convert.ToInt32(data["gymid"]),
                            SetDate = data["setdate"].ToString(),
                            RemoveDate = data["removedate"] == DBNull.Value ? null : data["removedate"].ToString(),
                            AdminID = Convert.ToInt32(data["adminid"])
                        });
                    }
                }
                return userRouteGrades;
            }
            finally
            {
                dbConn?.Close();
            }
        }

        public UserRouteGrade GetUserRouteGradeById(int userId, int routeId)
        {
            NpgsqlConnection dbConn = null;
            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();
                cmd.CommandText = "SELECT * FROM UserRouteGradeView WHERE userid = @userId AND routeid = @routeId";
                cmd.Parameters.Add("@userId", NpgsqlDbType.Integer).Value = userId;
                cmd.Parameters.Add("@routeId", NpgsqlDbType.Integer).Value = routeId;
                
                var data = GetData(dbConn, cmd);
                if (data != null)
                {
                    if (data.Read())
                    {
                        return new UserRouteGrade
                        {
                            UserID = Convert.ToInt32(data["userid"]),
                            RouteID = Convert.ToInt32(data["routeid"]),
                            GradeFbleau = data["gradefbleau"].ToString(),
                            Status = data["status"].ToString(),
                            GymID = Convert.ToInt32(data["gymid"]),
                            SetDate = data["setdate"].ToString(),
                            RemoveDate = data["removedate"] == DBNull.Value ? null : data["removedate"].ToString(),
                            AdminID = Convert.ToInt32(data["adminid"])
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

        public List<UserRouteGrade> GetUserRouteGradesByUserId(int userId)
        {
            NpgsqlConnection dbConn = null;
            var userRouteGrades = new List<UserRouteGrade>();
            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();
                cmd.CommandText = "SELECT * FROM UserRouteGradeView WHERE userid = @userId";
                cmd.Parameters.Add("@userId", NpgsqlDbType.Integer).Value = userId;
                
                var data = GetData(dbConn, cmd);
                if (data != null)
                {
                    while (data.Read())
                    {
                        userRouteGrades.Add(new UserRouteGrade
                        {
                            UserID = data["userid"] == DBNull.Value ? null : Convert.ToInt32(data["userid"]),
                            RouteID = Convert.ToInt32(data["routeid"]),
                            GradeFbleau = data["gradefbleau"].ToString(),
                            Status = data["status"].ToString(),
                            GymID = Convert.ToInt32(data["gymid"]),
                            SetDate = data["setdate"].ToString(),
                            RemoveDate = data["removedate"] == DBNull.Value ? null : data["removedate"].ToString(),
                            AdminID = Convert.ToInt32(data["adminid"])
                        });
                    }
                }
                return userRouteGrades;
            }
            finally
            {
                dbConn?.Close();
            }
        }

        public List<UserRouteGrade> GetUserRouteGradesByGymId(int gymId)
        {
            NpgsqlConnection dbConn = null;
            var userRouteGrades = new List<UserRouteGrade>();
            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();
                cmd.CommandText = "SELECT * FROM UserRouteGradeView WHERE gymid = @gymId";
                cmd.Parameters.Add("@gymId", NpgsqlDbType.Integer).Value = gymId;
                
                var data = GetData(dbConn, cmd);
                if (data != null)
                {
                    while (data.Read())
                    {
                        userRouteGrades.Add(new UserRouteGrade
                        {
                            UserID = data["userid"] == DBNull.Value ? null : Convert.ToInt32(data["userid"]),
                            RouteID = Convert.ToInt32(data["routeid"]),
                            GradeFbleau = data["gradefbleau"].ToString(),
                            Status = data["status"].ToString(),
                            GymID = Convert.ToInt32(data["gymid"]),
                            SetDate = data["setdate"].ToString(),
                            RemoveDate = data["removedate"] == DBNull.Value ? null : data["removedate"].ToString(),
                            AdminID = Convert.ToInt32(data["adminid"])
                        });
                    }
                }
                return userRouteGrades;
            }
            finally
            {
                dbConn?.Close();
            }
        }

        public List<UserRouteGrade> GetUserRouteGradesByUserIdAndRouteId(int userId, int routeId)
        {
            NpgsqlConnection dbConn = null;
            var userRouteGrades = new List<UserRouteGrade>();
            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();
                cmd.CommandText = "SELECT * FROM UserRouteGradeView WHERE userid = @userId AND routeid = @routeId";
                cmd.Parameters.Add("@userId", NpgsqlDbType.Integer).Value = userId;
                cmd.Parameters.Add("@routeId", NpgsqlDbType.Integer).Value = routeId;
                
                var data = GetData(dbConn, cmd);
                if (data != null)
                {
                    while (data.Read())
                    {
                        userRouteGrades.Add(new UserRouteGrade
                        {
                            UserID = data["userid"] == DBNull.Value ? null : Convert.ToInt32(data["userid"]),
                            RouteID = Convert.ToInt32(data["routeid"]),
                            GradeFbleau = data["gradefbleau"].ToString(),
                            Status = data["status"].ToString(),
                            GymID = Convert.ToInt32(data["gymid"]),
                            SetDate = data["setdate"].ToString(),
                            RemoveDate = data["removedate"] == DBNull.Value ? null : data["removedate"].ToString(),
                            AdminID = Convert.ToInt32(data["adminid"])
                        });
                    }
                }
                return userRouteGrades;
            }
            finally
            {
                dbConn?.Close();
            }
        }

        public bool InsertUserRouteGrade(UserRouteGrade userRouteGrade)
        {
            NpgsqlConnection dbConn = null;
            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();
                cmd.CommandText = "INSERT INTO UserRouteGradeView (userid, routeid, gradefbleau, status) VALUES (@userId, @routeId, @gradeFbleau, @status)";
                
                if (userRouteGrade.UserID.HasValue)
                {
                    cmd.Parameters.AddWithValue("@userId", NpgsqlDbType.Integer, userRouteGrade.UserID.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@userId", DBNull.Value);
                }
                
                cmd.Parameters.AddWithValue("@routeId", NpgsqlDbType.Integer, userRouteGrade.RouteID);
                cmd.Parameters.AddWithValue("@gradeFbleau", NpgsqlDbType.Varchar, userRouteGrade.GradeFbleau);
                cmd.Parameters.AddWithValue("@status", NpgsqlDbType.Varchar, userRouteGrade.Status);
                
                bool result = InsertData(dbConn, cmd);
                return result;
            }
            finally
            {
                dbConn?.Close();
            }
        }

        public bool UpdateUserRouteGrade(UserRouteGrade userRouteGrade)
        {
            var dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = "UPDATE UserRouteGradeView SET gradefbleau = @gradeFbleau, status = @status WHERE userid = @userId AND routeid = @routeId";
            
            if (userRouteGrade.UserID.HasValue)
            {
                cmd.Parameters.AddWithValue("@userId", NpgsqlDbType.Integer, userRouteGrade.UserID.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("@userId", DBNull.Value);
            }
            
            cmd.Parameters.AddWithValue("@routeId", NpgsqlDbType.Integer, userRouteGrade.RouteID);
            cmd.Parameters.AddWithValue("@gradeFbleau", NpgsqlDbType.Varchar, userRouteGrade.GradeFbleau);
            cmd.Parameters.AddWithValue("@status", NpgsqlDbType.Varchar, userRouteGrade.Status);
            
            bool result = UpdateData(dbConn, cmd);
            return result;
        }

        public bool DeleteUserRouteGrade(int userId, int routeId)
        {
            var dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = "DELETE FROM UserRouteGradeView WHERE userid = @userId AND routeid = @routeId";
            cmd.Parameters.AddWithValue("@userId", NpgsqlDbType.Integer, userId);
            cmd.Parameters.AddWithValue("@routeId", NpgsqlDbType.Integer, routeId);
            
            bool result = DeleteData(dbConn, cmd);
            return result;
        }
    }
}
