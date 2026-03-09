using System.Collections.Generic;
using System.Data.SqlClient;
using Front_Office_Concours_Admin.Models;

namespace Front_Office_Concours_Admin.Repository
{
    public class TypeEmploiRepository
    {
        private readonly string _connectionString;

        public TypeEmploiRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<TypeEmploi> GetAll()
        {
            var list = new List<TypeEmploi>();
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand("SELECT Id, Libelle FROM TypeEmploi", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new TypeEmploi()
                {
                    Id = (int)reader["Id"],
                    Libelle = reader["Libelle"].ToString()
                });
            }
            return list;
        }
    }
}