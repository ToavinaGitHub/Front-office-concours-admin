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
                           tc.Libelle AS TypeContrat, c.statutCandidature AS Statut,tc.Id AS TypeContrat_Id
                    FROM Candidature c
                    INNER JOIN Annonce a ON c.AnnonceId = a.Id
                    INNER JOIN Entite e ON a.entiteId = e.Id
                    INNER JOIN TypeContrat tc ON a.typeContratId = tc.Id
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
                                        
                                        Id =  reader.GetInt32(reader.GetOrdinal("TypeContrat_id")),
                                        Libelle = reader["TypeContrat"]?.ToString(),
                                    },
                                    Entite = new Entite
                                    {
                                        Nom = reader["NomEntite"]?.ToString()
                                    }
                                },

                                StatutCandidature = new StatutCandidature
                                {
                                    Id =  reader.GetInt32(reader.GetOrdinal("Statut")),
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
        
       public int CreateCandidature(Candidature candidature)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var transaction = conn.BeginTransaction();
            
            

            try
            {
                // Charger les exigences de l'annonce
                var exigences = new Dictionary<int, (bool Obligatoire, bool NeedPiece)>();

                string sqlExigences = @"
                    SELECT Id, IsObligatoire, NeedPieceJustificative
                    FROM Exigence
                    WHERE AnnonceId = @AnnonceId";

                using (var cmdEx = new SqlCommand(sqlExigences, conn, transaction))
                {
                    cmdEx.Parameters.AddWithValue("@AnnonceId", candidature.AnnonceId);
                    using var reader = cmdEx.ExecuteReader();
                    while (reader.Read())
                    {
                        exigences.Add(reader.GetInt32(0),
                            (reader.GetBoolean(1), reader.GetBoolean(2)));
                    }
                }

                // Vérification des exigences
                foreach (var ex in exigences)
                {
                    var detail = candidature.DetailsCandidature.FirstOrDefault(d => d.ExigenceId == ex.Key);

                    if (ex.Value.Obligatoire)
                    {
                        if (detail == null)
                            throw new ApplicationException("Une exigence obligatoire n'a pas été remplie.");
                    }

                    if (ex.Value.NeedPiece && detail?.Valeur == true)
                    {
                        if (string.IsNullOrEmpty(detail.PieceJustificative))
                            throw new ApplicationException("Une pièce justificative obligatoire est manquante.");
                    }
                }

                // Insérer la candidature
                string sqlCandidature = @"
                    INSERT INTO Candidature
                    (AnnonceId, CandidatId, CV, LettreMotivation, StatutCandidature, DateCreation)
                    VALUES
                    (@AnnonceId, @CandidatId, @CV, @LettreMotivation, @StatutCandidatureId, GETDATE());
                    SELECT SCOPE_IDENTITY();";

                using var cmd = new SqlCommand(sqlCandidature, conn, transaction);
                cmd.Parameters.AddWithValue("@AnnonceId", candidature.AnnonceId);
                cmd.Parameters.AddWithValue("@CandidatId", candidature.CandidatId);
                cmd.Parameters.AddWithValue("@CV", (object?)candidature.CV ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@LettreMotivation", (object?)candidature.LettreMotivation ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@StatutCandidatureId", candidature.StatutCandidatureId);

                int newCandidatureId = Convert.ToInt32(cmd.ExecuteScalar());

                // Insérer les détails
                foreach (var detail in candidature.DetailsCandidature)
                {
                    string sqlDetail = @"
                        INSERT INTO DetailsCandidature
                        (CandidatureId, ExigenceId, Valeur, PieceJustificative, DateCreation)
                        VALUES
                        (@CandidatureId, @ExigenceId, @Valeur, @PieceJustificative, GETDATE());";

                    using var cmdDetail = new SqlCommand(sqlDetail, conn, transaction);
                    cmdDetail.Parameters.AddWithValue("@CandidatureId", newCandidatureId);
                    cmdDetail.Parameters.AddWithValue("@ExigenceId", detail.ExigenceId);
                    cmdDetail.Parameters.AddWithValue("@Valeur", detail.Valeur);
                    cmdDetail.Parameters.AddWithValue("@PieceJustificative", (object?)detail.PieceJustificative ?? DBNull.Value);

                    cmdDetail.ExecuteNonQuery();
                }

                transaction.Commit();
                return newCandidatureId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }


    }
}
