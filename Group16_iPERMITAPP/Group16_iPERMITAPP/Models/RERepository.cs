using System.Data.SQLite;
using Group16_iPERMITAPP.Database;

namespace Group16_iPERMITAPP.Models
{
    public class RERepository
    {
        public RE GetById(string id, string password)
        {
            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();
                string query = "SELECT * FROM RE WHERE REId = @id AND REPassword = @password;";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@password", password);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new RE
                            {
                                REId = reader.GetString(0),
                                REPassword = reader.GetString(1),
                                ContactPersonName = reader.GetString(2),
                                CreatedDate = reader.GetString(3),
                                REEmail = reader.GetString(4),
                                OrganizationName = reader.GetString(5),
                                OrganizationAddress = reader.GetString(6)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public bool Register(RE re)
        {
            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();
                string query = @"INSERT INTO RE (REId, REPassword, contactPersonName, REEmail, organizationName, organizationAddress) 
                                VALUES (@id, @password, @contactPerson, @email, @orgName, @orgAddress);";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", re.REId);
                    cmd.Parameters.AddWithValue("@password", re.REPassword);
                    cmd.Parameters.AddWithValue("@contactPerson", re.ContactPersonName);
                    cmd.Parameters.AddWithValue("@email", re.REEmail);
                    cmd.Parameters.AddWithValue("@orgName", re.OrganizationName);
                    cmd.Parameters.AddWithValue("@orgAddress", re.OrganizationAddress);

                    try
                    {
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
        }
    }
}