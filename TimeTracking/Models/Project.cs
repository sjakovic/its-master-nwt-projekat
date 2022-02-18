using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TimeTracking.Data;

namespace TimeTracking.Models
{
    public partial class Project
    {
        public int Id { get; set; }

        [Required] 
        public string ProjectName { get; set; }

        public string ProjectDescription { get; set; }

        public void Load(int id)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = Startup.ConnectionString;

            try
            {
                using (conn)
                {
                    conn.Open();
                    string query = "SELECT * FROM projects WHERE Id = @Id";
                    SqlParameter pId = new SqlParameter("@Id", System.Data.SqlDbType.Int);
                    pId.Value = id;

                    SqlCommand command = new SqlCommand(query, conn);
                    command.Parameters.Add(pId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Id = Convert.ToInt32(reader["Id"]);
                            ProjectName = reader["ProjectName"].ToString();
                            ProjectDescription = reader["ProjectDescription"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        public static List<Project> GetAll()
        {
            List<Project> projects = new List<Project>();
            SqlConnection conn = new SqlConnection();

            conn.ConnectionString = Startup.ConnectionString;

            try
            {
                using (conn)
                {
                    conn.Open();
                    string query = "SELECT Id, ProjectName, ProjectDescription FROM projects";
                    SqlCommand command = new SqlCommand(query, conn);
                    Project project = null;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            project = new Project();
                            project.Id = Convert.ToInt32(reader["Id"]);
                            project.ProjectName = reader["ProjectName"].ToString();
                            project.ProjectDescription = reader["ProjectDescription"].ToString();
                            projects.Add(project);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Project project = new Project();
                project.ProjectName = ex.Message;
                projects.Add(project);
            }
            
            return projects;
        }

        public int AddProject()
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = Startup.ConnectionString;
            int rows = 0;
            try
            {
                using (conn)
                {
                    conn.Open();
                    string query = "INSERT INTO Projects(ProjectName,ProjectDescription)VALUES(@ProjectName,@ProjectDescription)";
                    SqlCommand command = new SqlCommand(query, conn);
                    SqlParameter pProjectName = new SqlParameter("@ProjectName", System.Data.SqlDbType.NVarChar);
                    pProjectName.Value = ProjectName;
                    SqlParameter pProjectDescription = new SqlParameter("@ProjectDescription", System.Data.SqlDbType.NVarChar);
                    pProjectDescription.Value = ProjectDescription;
                    command.Parameters.Add(pProjectName);
                    command.Parameters.Add(pProjectDescription);
                    rows = command.ExecuteNonQuery();


                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return rows;
        }

        public int UpdateProject()
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = Startup.ConnectionString;
            int rows = 0;
            try
            {
                using (conn)
                {
                    conn.Open();
                    string query = "UPDATE Projects SET ProjectName = @ProjectName, ProjectDescription = @ProjectDescription WHERE Id = @Id";
                    SqlCommand command = new SqlCommand(query, conn);
                    SqlParameter pId = new SqlParameter("@Id", System.Data.SqlDbType.Int);
                    pId.Value = Id;
                    SqlParameter pProjectName = new SqlParameter("@ProjectName", System.Data.SqlDbType.NVarChar);
                    pProjectName.Value = ProjectName;
                    SqlParameter pProjectDescription = new SqlParameter("@ProjectDescription", System.Data.SqlDbType.NVarChar);
                    pProjectDescription.Value = ProjectDescription;
                    command.Parameters.Add(pId);
                    command.Parameters.Add(pProjectName);
                    command.Parameters.Add(pProjectDescription);
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
                    string query = "DELETE FROM Projects WHERE Id = @Id";
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
