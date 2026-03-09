
using System.Data.SqlClient;
using System.Web;
using Front_Office_Concours_Admin.Models;
using Front_Office_Concours_Admin.Services;
using Microsoft.AspNetCore.WebUtilities;
using StatutAnnonce = dotnetProjectShared.Enums.StatutAnnonce;

namespace Front_Office_Concours_Admin.Repository;

public class AnnonceRepository : IAnnonceRepository
{
    private readonly string _connectionString;
    private readonly HttpClient _httpClient;
    

    public AnnonceRepository(IConfiguration configuration,HttpClient httpClient)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
        _httpClient = httpClient;  
    }

    public AnnoncePagedViewModel GetPagedAnnonces(int currentPage, int pageSize)
    {
        var result = new AnnoncePagedViewModel
        {
            CurrentPage = currentPage,
            PageSize = pageSize
        };

        int offset = (currentPage - 1) * pageSize;

        using var conn = new SqlConnection(_connectionString);
        conn.Open();

        // 🔢 total
        using (var countCmd = new SqlCommand("SELECT COUNT(*) FROM Annonce", conn))
        {
            result.TotalItems = (int)countCmd.ExecuteScalar();
        }

        // 📄 données avec JOIN
        using var cmd = new SqlCommand(@"
            SELECT
                a.Id,
                a.Titre,
                a.Description,
                a.lieuPoste,
                a.DateCreation,
                a.DateLimiteDepotDossier,
                a.DateConcours,
                a.TachesPrincipales,

                e.Id AS EntiteId,
                e.Nom AS EntiteNom,

                sa.Id AS StatutAnnonceId,
                sa.Libelle AS StatutAnnonceLibelle,

                tc.Id AS TypeContratId,
                tc.Libelle AS TypeContratLibelle,

                te.Id AS TypeEmploiId,
                te.Libelle AS TypeEmploiLibelle
            FROM Annonce a
            INNER JOIN Entite e ON e.Id = a.entiteId
            INNER JOIN StatutAnnonce sa ON sa.Id = a.statutAnnonceId
            LEFT JOIN TypeContrat tc ON tc.Id = a.typeContratId
            LEFT JOIN TypeEmploi te ON te.Id = a.typeEmploiId
            ORDER BY a.DateCreation DESC
            OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;", conn);

        cmd.Parameters.AddWithValue("@offset", offset);
        cmd.Parameters.AddWithValue("@pageSize", pageSize);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var annonce = new Annonce
            {
                Id = (int)reader["Id"],
                Titre = reader["Titre"].ToString(),
                Description = reader["Description"] as string,
                LieuPoste = reader["lieuPoste"].ToString(),

                DateCreation = (DateTime)reader["DateCreation"],
                DateLimiteDepotDossier = (DateTime)reader["DateLimiteDepotDossier"],

                DateConcours = reader["DateConcours"] as DateTime?,
                TachesPrincipales = reader["TachesPrincipales"] as string,

                EntiteId = (int)reader["EntiteId"],
                StatutAnnonceId = (int)reader["StatutAnnonceId"],
                TypeContratId = reader["TypeContratId"] as int?,
                TypeEmploiId = reader["TypeEmploiId"] as int?
            };

            // 🔗 objets liés (MANUEL)
            annonce.Entite = new Entite
            {
                Id = (int)reader["EntiteId"],
                Nom = reader["EntiteNom"].ToString(),
            };

            var enumService = new EnumsService();
            
            int statutId = (int)reader["StatutAnnonce"];

            // utiliser l'enum Shared
           // string libelle = enumService.GetDescriptionFromEnumValue<StatutAnnonce>(statutId);

            // créer l'objet StatutAnnonce (classe) pour ton annonce
            annonce.StatutAnnonce = new Front_Office_Concours_Admin.Models.StatutAnnonce
            {
                Id = statutId,
                Libelle = ""
            };

            if (reader["TypeContratId"] != DBNull.Value)
            {
                annonce.TypeContrat = new TypeContrat
                {
                    Id = (int)reader["TypeContratId"],
                    Libelle = reader["TypeContratLibelle"].ToString()
                };
            }

            if (reader["TypeEmploiId"] != DBNull.Value)
            {
                annonce.TypeEmploi = new TypeEmploi
                {
                    Id = (int)reader["TypeEmploiId"],
                    Libelle = reader["TypeEmploiLibelle"].ToString()
                };
            }

            result.Annonces.Add(annonce);
        }

        return result;
    }
    
    public AnnoncePagedViewModel GetPagedAnnoncesByIds(List<int> ids, int currentPage, int pageSize)
    {
        var result = new AnnoncePagedViewModel
        {
            CurrentPage = currentPage,
            PageSize = pageSize,
            TotalItems = ids.Count
        };

        if (!ids.Any()) return result;

        var idsParam = string.Join(",", ids); // Ex: "1,5,12"

        using var conn = new SqlConnection(_connectionString);
        conn.Open();

        // 📄 données avec JOIN et filtre sur IDs
        using var cmd = new SqlCommand($@"
            SELECT
                a.Id,
                a.Titre,
                a.Description,
                a.lieuPoste,
                a.DateCreation,
                a.DateLimiteDepotDossier,
                a.DateConcours,
                a.TachesPrincipales,

                e.Id AS EntiteId,
                e.Nom AS EntiteNom,

                sa.Id AS StatutAnnonceId,
                sa.Libelle AS StatutAnnonceLibelle,

                tc.Id AS TypeContratId,
                tc.Libelle AS TypeContratLibelle,

                te.Id AS TypeEmploiId,
                te.Libelle AS TypeEmploiLibelle
            FROM Annonce a
            INNER JOIN Entite e ON e.Id = a.entiteId
            INNER JOIN StatutAnnonce sa ON sa.Id = a.statutAnnonceId
            LEFT JOIN TypeContrat tc ON tc.Id = a.typeContratId
            LEFT JOIN TypeEmploi te ON te.Id = a.typeEmploiId
            WHERE a.Id IN ({idsParam})
            ORDER BY a.DateCreation DESC
            OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;", conn);

        cmd.Parameters.AddWithValue("@offset", (currentPage - 1) * pageSize);
        cmd.Parameters.AddWithValue("@pageSize", pageSize);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var annonce = new Annonce
            {
                Id = (int)reader["Id"],
                Titre = reader["Titre"].ToString(),
                Description = reader["Description"] as string,
                LieuPoste = reader["lieuPoste"].ToString(),

                DateCreation = (DateTime)reader["DateCreation"],
                DateLimiteDepotDossier = (DateTime)reader["DateLimiteDepotDossier"],
                DateConcours = reader["DateConcours"] as DateTime?,
                TachesPrincipales = reader["TachesPrincipales"] as string,

                EntiteId = (int)reader["EntiteId"],
                StatutAnnonceId = (int)reader["StatutAnnonceId"],
                TypeContratId = reader["TypeContratId"] as int?,
                TypeEmploiId = reader["TypeEmploiId"] as int?
            };

            // 🔗 Objets liés
            annonce.Entite = new Entite
            {
                Id = (int)reader["EntiteId"],
                Nom = reader["EntiteNom"].ToString(),
            };

            

             var enumService = new EnumsService();


            int statutId = (int)reader["StatutAnnonce"];

            //string libelle = enumService.GetDescriptionFromEnumValue<dotnetProjectShared.Enums.StatutAnnonce>(statutId);

            // créer l'objet StatutAnnonce (classe) pour ton annonce
            annonce.StatutAnnonce = new Front_Office_Concours_Admin.Models.StatutAnnonce
            {
                Id = statutId,
                Libelle = ""
            };


            if (reader["TypeContratId"] != DBNull.Value)
            {
                annonce.TypeContrat = new TypeContrat
                {
                    Id = (int)reader["TypeContratId"],
                    Libelle = reader["TypeContratLibelle"].ToString()
                };
            }

            if (reader["TypeEmploiId"] != DBNull.Value)
            {
                annonce.TypeEmploi = new TypeEmploi
                {
                    Id = (int)reader["TypeEmploiId"],
                    Libelle = reader["TypeEmploiLibelle"].ToString()
                };
            }

            result.Annonces.Add(annonce);
        }

        return result;
    }

   public DetailsCandidatureResponse GetDetailsCandidatureById(int candidatureId)
    {
        DetailsCandidatureResponse result = null;

        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();

            string queryDetails = @"
                SELECT 
                    c.candidatId candidat_id,
                    a.Titre AS titre_poste,
                    a.lieuPoste AS lieu_poste,
                    tc.Libelle AS type_contrat,
                    te.Libelle AS type_emploi,
                    a.DateCreation AS date_creation,
                    a.Description AS poste_description,
                    c.DateCreation AS postule_date,
                    e.Nom as nom_entite,
                    c.statutCandidature as statut_id,
                    a.tachesPrincipales as taches_principales
                FROM Candidature c
                JOIN Annonce a ON c.annonceId = a.Id
                JOIN Entite e ON a.entiteId = e.Id
                JOIN TypeContrat tc ON tc.Id = a.typeContratId
                JOIN TypeEmploi te ON te.Id = a.typeEmploiId
                WHERE c.Id = @CandidatureId";

            

            var enumsService = new EnumsService();
            
            using (SqlCommand cmd = new SqlCommand(queryDetails, conn))
            {
                cmd.Parameters.AddWithValue("@CandidatureId", candidatureId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        result = new DetailsCandidatureResponse
                        {
                            CandidatID = reader.GetInt32(reader.GetOrdinal("candidat_id")),
                            TitrePoste = reader.GetString(reader.GetOrdinal("titre_poste")),
                            LieuPoste = reader.GetString(reader.GetOrdinal("lieu_poste")),
                            TypeContrat = reader.GetString(reader.GetOrdinal("type_contrat")),
                            TypeEmploi = reader.GetString(reader.GetOrdinal("type_emploi")),
                            DateCreationPoste = reader.GetDateTime(reader.GetOrdinal("date_creation")),
                            PosteDescription = reader.GetString(reader.GetOrdinal("poste_description")),
                            DatePostulation = reader.GetDateTime(reader.GetOrdinal("postule_date")),
                            Statut = enumsService.GetDescriptionFromEnumValue(typeof(dotnetProjectShared.Enums.StatutCandidature), reader.GetInt32(reader.GetOrdinal("statut_id"))),
                            NomEntite =  reader.GetString(reader.GetOrdinal("nom_entite")),
                            Statut_ID =  reader.GetInt32(reader.GetOrdinal("statut_id")),
                            TachesPrincipale = reader.GetString(reader.GetOrdinal("taches_principales")),
                            taches = reader.GetString(reader.GetOrdinal("taches_principales")).Split(',') ?? new string[] { }
                        };
                    }
                }
            }

            if (result == null)
                return null;

            string queryExigences = @"
               SELECT
                    e.Id AS id,
                    e.Libelle AS libelle,
                    CAST(
                            COALESCE(MAX(CAST(dc.Valeur AS int)), 0)
                        AS bit
                    ) AS Valeur
                FROM Exigence e
                         LEFT JOIN DetailsCandidature dc
                                   ON e.Id = dc.exigenceId
                WHERE e.annonceId = (
                    SELECT annonceId FROM Candidature WHERE Id = @CandidatureId
                )
                GROUP BY e.Id, e.Libelle;
            ";
    
            using (SqlCommand cmd = new SqlCommand(queryExigences, conn))
            {
                cmd.Parameters.AddWithValue("@CandidatureId", candidatureId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Exigences.Add(new ExigenceCandidatureDto
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Libelle = reader.GetString(reader.GetOrdinal("libelle")),
                            Valeur = reader.GetBoolean(reader.GetOrdinal("Valeur"))
                        });
                    }
                }
            }
        }

        return result;
    }
   
   public DetailsAnnonceResponse GetDetailsAnnonceById(int annonceId)
    {
        DetailsAnnonceResponse result = null;

        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();

            string queryDetails = @"
                SELECT 
                    a.Titre AS titre_poste,
                    a.lieuPoste AS lieu_poste,
                    tc.Libelle AS type_contrat,
                    te.Libelle AS type_emploi,
                    a.DateCreation AS date_creation,
                    a.Description AS poste_description,
                    e.Nom as nom_entite,
                    a.dateLimiteDepotDossier as dateLimiteDepotDossier,
                    a.statutAnnonce AS statut,
                    a.tachesPrincipales as tachesPrincipales,
                    a.id as annonce_id,
                    te.id as type_emploi_id
                FROM Annonce a 
                JOIN Entite e ON a.entiteId = e.Id
                JOIN TypeContrat tc ON tc.Id = a.typeContratId
                JOIN TypeEmploi te ON te.Id = a.typeEmploiId
                WHERE a.id = @annonceId";

            using (SqlCommand cmd = new SqlCommand(queryDetails, conn))
            {
                cmd.Parameters.AddWithValue("@annonceId", annonceId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        result = new DetailsAnnonceResponse
                        {
                            TitrePoste = reader.GetString(reader.GetOrdinal("titre_poste")),
                            LieuPoste = reader.GetString(reader.GetOrdinal("lieu_poste")),
                            TypeContrat = reader.GetString(reader.GetOrdinal("type_contrat")),
                            TypeEmploi = reader.GetString(reader.GetOrdinal("type_emploi")),
                            DateCreationPoste = reader.GetDateTime(reader.GetOrdinal("date_creation")),
                            PosteDescription = reader.GetString(reader.GetOrdinal("poste_description")),
                            DateLimiteDepotDossier = reader.GetDateTime(reader.GetOrdinal("dateLimiteDepotDossier")),
                            Statut = reader.GetInt32(reader.GetOrdinal("statut")),
                            NomEntite =  reader.GetString(reader.GetOrdinal("nom_entite")),
                            TachesPrincipales = reader.GetString(reader.GetOrdinal("tachesPrincipales")),
                            taches = reader.GetString(reader.GetOrdinal("tachesPrincipales")).Split(',') ?? new string[] { },
                            Annonce_ID =  reader.GetInt32(reader.GetOrdinal("annonce_id")),
                            TypeEmploi_ID =  reader.GetInt32(reader.GetOrdinal("type_emploi_id")),
                        };
                    }
                }
            }

            if (result == null)
                return null;
            string queryExigences = @"
                SELECT
                    e.Id AS id,
                    e.Libelle AS libelle,
                    e.isObligatoire as isObligatoire,
                    e.needPieceJustificative as needPieceJustificative
                FROM Exigence e
                WHERE annonceId = @annonceId";
    
            using (SqlCommand cmd = new SqlCommand(queryExigences, conn))
            {
                cmd.Parameters.AddWithValue("@annonceId", annonceId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Exigences.Add(new ExigenceAnnonceDto
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Libelle = reader.GetString(reader.GetOrdinal("libelle")),
                            IsObligatoire = reader.GetBoolean(reader.GetOrdinal("isObligatoire")),
                            NeedPieceJustificative = reader.GetBoolean(reader.GetOrdinal("needPieceJustificative"))
                        });
                    }
                }
            }
        }

        return result;
    }

    public bool CheckIfUserAlreadyApply(int userId, int annonceId)
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            string query = @"
            SELECT 1
            FROM Candidature
            WHERE annonceId = @annonceId
              AND candidatId = @candidatId";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@annonceId", annonceId);
                cmd.Parameters.AddWithValue("@candidatId", userId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    return reader.HasRows;
                }
            }
        }
    }
    
    public async Task<List<AnnonceElasticSearchResult>> SearchAsync(
        string? titre,
        string? lieu,
        int? contratId,
        int? emploiId,
        string? sortOrder,
        int page,
        int size)
    {
        var query = new Dictionary<string, string?>
        {
            ["titre"] = titre,
            ["lieu"] = lieu,
            ["contratId"] = contratId > 0 ? contratId.ToString() : null,
            ["emploiId"] = emploiId > 0 ? emploiId.ToString() : null,
            ["page"] = page.ToString(),
            ["size"] = size.ToString(),
            ["sortOrder"] = sortOrder ?? "dateDesc"
        };

        var url = QueryHelpers.AddQueryString("/api/annonces/search", query!);

        var pagedResult = await _httpClient.GetFromJsonAsync<PagedAnnonceResult>(url);

        return pagedResult?.Annonces ?? new List<AnnonceElasticSearchResult>();
    }
}

   