namespace Front_Office_Concours_Admin.Models
{
    public class ExigenceAnnonceDto
    {
        public int? Id { get; set; }
        public string? Libelle { get; set; }
        
        public bool IsObligatoire  { get; set; }
        
        public bool NeedPieceJustificative { get; set; }
        
        
    }
}