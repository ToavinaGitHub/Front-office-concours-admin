namespace Front_Office_Concours_Admin.Models
{
    public class AnnonceElasticSearchResult
    {
        public int Id { get; set; }

        public string Titre { get; set; }

        public string Description { get; set; }

        public string TypeContrat { get; set; }
        public int TypeContratId { get; set; }

        public string TypeEmploi { get; set; } 
        public int TypeEmploiId { get; set; }

        public string StatutAnnonce { get; set; }
        public int StatutAnnonceId { get; set; }

        public string Lieu { get; set; }

        public DateTime DatePublication { get; set; }

        public int EntiteId { get; set; }
        public string EntiteNom { get; set; }

        public bool HasConcours { get; set; }
    }
}