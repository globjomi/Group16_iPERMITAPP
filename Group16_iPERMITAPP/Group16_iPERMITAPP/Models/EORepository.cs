using System.Collections.Generic;
using System.Data.SQLite;
using Group16_iPERMITAPP.Database;

namespace Group16_iPERMITAPP.Models
{
    public class EORepository
    {
        public List<EO> GetAll()
        {
            var eoList = new List<EO>();

            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();
                string query = "SELECT * FROM EO;";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            eoList.Add(new EO
                            {
                                EOId = reader.GetInt32(0),
                                EOName = reader.GetString(1),
                                EOPassword = reader.GetString(2)
                            });
                        }
                    }
                }
            }

            return eoList;
        }

        public EO GetById(string id, string password)
        {
            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();
                string query = "SELECT * FROM EO WHERE EOId = @id AND EOPassword = @password;";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@password", password);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new EO
                            {
                                EOId = reader.GetInt32(0),
                                EOName = reader.GetString(1),
                                EOPassword = reader.GetString(2)
                            };
                        }
                    }
                }
            }
            return null;
        }
    }
}