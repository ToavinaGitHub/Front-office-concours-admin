
using System.Data.SqlClient;
using Front_Office_Concours_Admin.Models;

namespace Front_Office_Concours_Admin.Repository;

public class AnnonceRepository : IAnnonceRepository
{
    private readonly string _connectionString;

    public AnnonceRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
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

            annonce.StatutAnnonce = new StatutAnnonce
            {
                Id = (int)reader["StatutAnnonceId"],
                Libelle = reader["StatutAnnonceLibelle"].ToString()
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

}