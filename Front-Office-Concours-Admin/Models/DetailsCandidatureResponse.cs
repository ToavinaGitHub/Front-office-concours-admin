namespace Front_Office_Concours_Admin.Models
{
    public class DetailsCandidatureResponse : DetailsAnnonceResponse
    {
        public string TitrePoste { get; set; }
        public string LieuPoste { get; set; }
        public string TypeContrat { get; set; }
        public string TypeEmploi { get; set; }
        public DateTime DateCreationPoste { get; set; }
        public string PosteDescription { get; set; }
        public DateTime DatePostulation { get; set; }
        public string Statut { get; set; }
        public int Statut_ID { get; set; }
        public int CandidatID { get; set; }
        
        public string TachesPrincipale { get; set; }
        
        public string NomEntite { get; set; }
        
        public string[]? taches { get; set; }

        public List<ExigenceCandidatureDto> Exigences { get; set; } = new();
    }
}