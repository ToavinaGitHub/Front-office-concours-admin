using System.Collections.Generic;
using System.Data.SqlClient;
using Front_Office_Concours_Admin.Models;

namespace Front_Office_Concours_Admin.Repository
{
    public class TypeContratRepository
    {
        private readonly string _connectionString;

        public TypeContratRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<TypeContrat> GetAll()
        {
            var list = new List<TypeContrat>();
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = new SqlCommand("SELECT Id, Libelle FROM TypeContrat", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new TypeContrat
                {
                    Id = (int)reader["Id"],
                    Libelle = reader["Libelle"].ToString()
                });
            }

            return list;
        }
    }
}