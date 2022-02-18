using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TimeTracking.Models
{
    public class TimeTable
    {
        public int Id { get; set; }

        [Required]
        public int ProjectId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public DateTime WorkDate { get; set; }

        [Required]
        public int WorkTime { get; set; }

        public string UserName { get; set; }

        public void Load(int id)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = Startup.ConnectionString;

            try
            {
                using (conn)
                {
                    conn.Open();
                    string query = "SELECT * FROM TimeTable WHERE Id = @Id";
                    SqlParameter pId = new SqlParameter("@Id", System.Data.SqlDbType.Int);
                    pId.Value = id;

                    SqlCommand command = new SqlCommand(query, conn);
                    command.Parameters.Add(pId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Id = Convert.ToInt32(reader["Id"]);
                            UserId = reader["UserId"].ToString();
                            ProjectId = Convert.ToInt32(reader["ProjectId"]);
                            WorkDate = (DateTime)reader["WorkDate"];
                            WorkTime = Convert.ToInt32(reader["WorkTime"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public static List<TimeTable> GetAll(AspNetUser user, Project project)
        {
            List<TimeTable> timeTables = new List<TimeTable>();
            SqlConnection conn = new SqlConnection();
            string query = "";
            
            conn.ConnectionString = Startup.ConnectionString;

            try
            {
                using (conn)
                {
                    conn.Open();
                    query = "SELECT tt.Id, tt.ProjectId, tt.UserId, tt.WorkDate, tt.WorkTime, u.UserName FROM TimeTable tt INNER JOIN AspNetUsers u ON tt.UserId = u.Id and tt.ProjectId = @ProjectId ORDER BY u.UserName ASC, tt.WorkDate ASC";
                    if(!user.HasRole("admin")) 
                        query = "SELECT tt.Id, tt.ProjectId, tt.UserId, tt.WorkDate, tt.WorkTime, u.UserName FROM TimeTable tt INNER JOIN AspNetUsers u ON tt.UserId = u.Id WHERE tt.UserId = @UserId and tt.ProjectId = @ProjectId ORDER BY u.UserName ASC, tt.WorkDate ASC";


                    SqlCommand command = new SqlCommand(query, conn);
                    if(!user.HasRole("admin"))
                    {
                        SqlParameter pUserId = new SqlParameter("@UserId", System.Data.SqlDbType.NVarChar);
                        pUserId.Value = user.Id;
                        command.Parameters.Add(pUserId);
                    }
                    SqlParameter pProjectId = new SqlParameter("@ProjectId", System.Data.SqlDbType.Int);
                    pProjectId.Value = project.Id;
                    command.Parameters.Add(pProjectId);

                    TimeTable timeTable = null;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            timeTable = new TimeTable();
                            timeTable.Id = Convert.ToInt32(reader["Id"]);
                            timeTable.ProjectId = Convert.ToInt32(reader["ProjectId"]);
                            timeTable.UserId = reader["UserId"].ToString();
                            timeTable.WorkDate = (DateTime)reader["WorkDate"];
                            timeTable.WorkTime = Convert.ToInt32(reader["WorkTime"]);
                            timeTable.UserName = reader["UserName"].ToString();
                            timeTables.Add(timeTable);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

            return timeTables;
        }

        public int save()
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = Startup.ConnectionString;
            int rows = 0;
            try
            {
                using (conn)
                {
                    conn.Open();
                    string query = "INSERT INTO TimeTable(UserId,ProjectId,WorkDate,WorkTime)VALUES(@UserId,@ProjectId,@WorkDate,@WorkTime)";
                    SqlCommand command = new SqlCommand(query, conn);
                    
                    SqlParameter pUserId = new SqlParameter("@UserId", System.Data.SqlDbType.NVarChar);
                    pUserId.Value = UserId;
                    SqlParameter pProjectId = new SqlParameter("@ProjectId", System.Data.SqlDbType.Int);
                    pProjectId.Value = ProjectId;
                    SqlParameter pWorkDate = new SqlParameter("@WorkDate", System.Data.SqlDbType.DateTime);
                    pWorkDate.Value = WorkDate;
                    SqlParameter pWorkTime = new SqlParameter("@WorkTime", System.Data.SqlDbType.Int);
                    pWorkTime.Value = WorkTime;

                    command.Parameters.Add(pUserId);
                    command.Parameters.Add(pProjectId);
                    command.Parameters.Add(pWorkDate);
                    command.Parameters.Add(pWorkTime);

                    rows = command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return rows;
        }
        public int Delete()
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = Startup.ConnectionString;
            int rows = 0;
            try
            {
                using (conn)
                {
                    conn.Open();
                    string query = "DELETE FROM TimeTable WHERE Id = @Id";
                    SqlCommand command = new SqlCommand(query, conn);
                    SqlParameter pId = new SqlParameter("@Id", System.Data.SqlDbType.Int);
                    pId.Value = Id;
                    command.Parameters.Add(pId);
                    rows = command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return rows;
        }

    }
}
