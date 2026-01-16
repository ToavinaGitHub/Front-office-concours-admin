namespace Front_Office_Concours_Admin.Models
{
    public class DetailsAnnonceResponse
    {
        public int Annonce_ID { get; set; } 
        public string TitrePoste { get; set; }
        public string LieuPoste { get; set; }
        public string TypeContrat { get; set; }
        public string TypeEmploi { get; set; }
        public DateTime DateCreationPoste { get; set; }
        public string PosteDescription { get; set; }
        public string Statut { get; set; }
        public int Statut_ID { get; set; }
        
        public string TachesPrincipales { get; set; }
        
        public DateTime DateLimiteDepotDossier { get; set; }
        
        public string NomEntite { get; set; }
        
        public string[]? taches { get; set; }
        
        public List<ExigenceAnnonceDto> Exigences { get; set; } = new();
    }
}