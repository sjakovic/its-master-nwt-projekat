using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace TimeTracking.Models
{
    public class AspNetUser
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }

        public void LoadByEmail(string email)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = Startup.ConnectionString;

            try
            {
                using (conn)
                {
                    conn.Open();
                    string query = "SELECT u.Id, u.UserName, u.Email, r.Name as 'RoleName' FROM AspNetUsers u INNER JOIN AspNetUserRoles ur ON  ur.UserId = u.Id INNER JOIN AspNetRoles r ON r.Id = ur.RoleId WHERE u.Email = @Email";
                    SqlParameter pEmail = new SqlParameter("@Email", SqlDbType.NVarChar);
                    pEmail.Value = email;

                    SqlCommand command = new SqlCommand(query, conn);
                    command.Parameters.Add(pEmail);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Id = reader["Id"].ToString();
                            UserName = reader["UserName"].ToString();
                            Email = reader["Email"].ToString();
                            Role = reader["RoleName"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public bool HasRole(string roleName)
        {
            return Role == roleName;
        }

    }
}
