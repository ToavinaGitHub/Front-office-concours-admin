using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Front_Office_Concours_Admin.Models;

namespace Front_Office_Concours_Admin.Repository
{
    public class CandidatureRepository
    {
        private readonly string _connectionString;

        public CandidatureRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // Récupérer candidatures du candidat connecté avec recherche, filtre typeContrat et pagination
        public (List<Candidature> Candidatures, int TotalCount) GetCandidatures(
            int candidatId, string keyword = "", string typeContratId = "",
            int pageNumber = 1, int pageSize = 10)
        {
            var result = new List<Candidature>();
            int totalCount = 0;

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                int offset = (pageNumber - 1) * pageSize;

                string sql = @"
-- Récupération paginée
SELECT c.Id, c.AnnonceId, c.CandidatId, c.DateCreation, c.CV, c.LettreMotivation,
       a.Titre AS AnnonceTitre, e.Nom AS NomEntite, a.lieuPoste AS LieuPoste,
       tc.Libelle AS TypeContrat, s.Libelle AS Statut
FROM Candidature c
INNER JOIN Annonce a ON c.AnnonceId = a.Id
INNER JOIN Entite e ON a.entiteId = e.Id
INNER JOIN TypeContrat tc ON a.typeContratId = tc.Id
INNER JOIN StatutCandidature s ON c.StatutCandidatureId = s.Id
WHERE c.CandidatId = @candidatId
  AND (@keyword = '' OR a.Titre LIKE '%' + @keyword + '%' OR a.lieuPoste LIKE '%' + @keyword + '%')
  AND (@typeContratId = '' OR a.typeContratId = @typeContratId)
ORDER BY c.DateCreation DESC
OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;

-- Total count
SELECT COUNT(*) 
FROM Candidature c
INNER JOIN Annonce a ON c.AnnonceId = a.Id
WHERE c.CandidatId = @candidatId
  AND (@keyword = '' OR a.Titre LIKE '%' + @keyword + '%' OR a.lieuPoste LIKE '%' + @keyword + '%')
  AND (@typeContratId = '' OR a.typeContratId = @typeContratId);
";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@candidatId", candidatId);
                    cmd.Parameters.AddWithValue("@keyword", keyword ?? "");
                    cmd.Parameters.AddWithValue("@typeContratId", typeContratId ?? "");
                    cmd.Parameters.AddWithValue("@offset", offset);
                    cmd.Parameters.AddWithValue("@pageSize", pageSize);

                    using (var reader = cmd.ExecuteReader())
                    {
                        // Lecture des candidatures
                        while (reader.Read())
                        {
                            var candidature = new Candidature
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                AnnonceId = Convert.ToInt32(reader["AnnonceId"]),
                                CandidatId = Convert.ToInt32(reader["CandidatId"]),
                                DateCreation = Convert.ToDateTime(reader["DateCreation"]),
                                CV = reader["CV"]?.ToString(),
                                LettreMotivation = reader["LettreMotivation"]?.ToString(),

                                Annonce = new Annonce
                                {
                                    Id = Convert.ToInt32(reader["AnnonceId"]),
                                    Titre = reader["AnnonceTitre"]?.ToString(),
                                    LieuPoste = reader["LieuPoste"]?.ToString(),
                                    TypeContrat = new TypeContrat
                                    {
                                        Libelle = reader["TypeContrat"]?.ToString(),
                                    },
                                    Entite = new Entite
                                    {
                                        Nom = reader["NomEntite"]?.ToString()
                                    }
                                },

                                StatutCandidature = new StatutCandidature
                                {
                                    Libelle = reader["Statut"]?.ToString()
                                }
                            };

                            result.Add(candidature);
                        }

                        // Lecture du total count
                        if (reader.NextResult() && reader.Read())
                        {
                            totalCount = Convert.ToInt32(reader[0]);
                        }
                    }
                }
            }

            return (result, totalCount);
        }

        // Récupérer candidature par Id
        public Candidature GetCandidatureById(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sql = @"
                    SELECT c.Id, c.AnnonceId, c.CandidatId, c.DateCreation, c.CV, c.LettreMotivation,
                           a.Titre AS AnnonceTitre, e.Nom AS NomEntite, a.lieuPoste AS LieuPoste,
                           tc.Libelle AS TypeContrat, s.Libelle AS Statut
                    FROM Candidature c
                    INNER JOIN Annonce a ON c.AnnonceId = a.Id
                    INNER JOIN Entite e ON a.entiteId = e.Id
                    INNER JOIN TypeContrat tc ON a.typeContratId = tc.Id
                    INNER JOIN StatutCandidature s ON c.StatutCandidatureId = s.Id
                    WHERE c.Id = @id
                    ";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Candidature
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                AnnonceId = Convert.ToInt32(reader["AnnonceId"]),
                                CandidatId = Convert.ToInt32(reader["CandidatId"]),
                                DateCreation = Convert.ToDateTime(reader["DateCreation"]),
                                CV = reader["CV"]?.ToString(),
                                LettreMotivation = reader["LettreMotivation"]?.ToString(),

                                Annonce = new Annonce
                                {
                                    Id = Convert.ToInt32(reader["AnnonceId"]),
                                    Titre = reader["AnnonceTitre"]?.ToString(),
                                    LieuPoste = reader["LieuPoste"]?.ToString(),
                                    TypeContrat = new TypeContrat
                                    {
                                        Libelle = reader["TypeContrat"]?.ToString(),
                                    },
                                    Entite = new Entite
                                    {
                                        Nom = reader["NomEntite"]?.ToString()
                                    }
                                },

                                StatutCandidature = new StatutCandidature
                                {
                                    Libelle = reader["Statut"]?.ToString()
                                }
                            };
                        }
                    }
                }
            }

            return null;
        }
    }
}
