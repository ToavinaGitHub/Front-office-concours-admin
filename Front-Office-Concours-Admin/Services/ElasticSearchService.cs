using Nest;
using Front_Office_Concours_Admin.Models;

namespace Front_Office_Concours_Admin.Services
{
    public class ElasticSearchService
    {
        private readonly ElasticClient _client;

        public ElasticSearchService(IConfiguration config)
        {
            var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
                .ServerCertificateValidationCallback((o, certificate, chain, errors) => true) // Ignore SSL errors
                .BasicAuthentication("elastic", "T_xxOKtkwWlPVxHeIRS4") 
                .DefaultIndex("annonces");

            _client = new ElasticClient(settings);

        }

        // Indexer une annonce (à faire pour toutes les annonces existantes)
        public void IndexAnnonce(Annonce annonce)
        {
            _client.IndexDocument(new
            {
                id = annonce.Id,
                titre = annonce.Titre,
                description = annonce.Description,
                lieuPoste = annonce.LieuPoste,
                typeContrat = annonce.TypeContrat != null ? annonce.TypeContrat.Libelle : "",
                horaire = annonce.TypeEmploi != null ? annonce.TypeEmploi.Libelle : "",
                dateCreation = annonce.DateCreation
            });
        }


        // Recherche avec filtres
        public List<int> SearchAnnonceIds(
            string title, string location, string typeContrat, string diplome, string horaire,
            int currentPage, int pageSize)
        {
            // 🔹 Requête vers Elasticsearch
            var response = _client.Search<Dictionary<string, object>>(s => s
                .From((currentPage - 1) * pageSize)
                .Size(pageSize)
                .Query(q => q
                    .Bool(b => b
                        .Must(
                            string.IsNullOrEmpty(title) ? null : m => m.Match(mm => mm.Field("titre").Query(title))
                        )
                        .Filter(
                            string.IsNullOrEmpty(location) ? null : f => f.Term("lieuPoste", location),
                            string.IsNullOrEmpty(typeContrat) ? null : f => f.Term("typeContrat", typeContrat),
                            string.IsNullOrEmpty(diplome) ? null : f => f.Term("diplome", diplome),
                            string.IsNullOrEmpty(horaire) ? null : f => f.Term("horaire", horaire)
                        )
                    )
                )
            );

            // 🔹 Debug info complète
            Console.WriteLine("===== DEBUG ELASTICSEARCH =====");
            Console.WriteLine(response.DebugInformation);
            Console.WriteLine("Total hits: " + response.Hits.Count);

            // 🔹 Lister tous les hits et leurs clés
            foreach (var hit in response.Hits)
            {
                Console.WriteLine("Hit source keys: " + string.Join(", ", hit.Source.Keys));
                if (hit.Source.ContainsKey("id"))
                {
                    Console.WriteLine("Found id: " + hit.Source["id"]);
                }
                else
                {
                    Console.WriteLine("No id field found in this hit!");
                }
            }

            // 🔹 Retourne la liste des IDs si elle existe
            return response.Hits
                .Where(h => h.Source.ContainsKey("id"))
                .Select(h => Convert.ToInt32(h.Source["id"]))
                .ToList();
        }

    }
}
