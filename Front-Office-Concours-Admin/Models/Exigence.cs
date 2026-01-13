namespace Front_Office_Concours_Admin.Models;

public class Exigence
{
    public int Id { get; set; }

    public int AnnonceId { get; set; }
    public Annonce Annonce { get; set; }

    public string Libelle { get; set; }

    public int Poids { get; set; }
    public bool IsObligatoire { get; set; } = false;
    public bool NeedPieceJustificative { get; set; } = false;

    public List<DetailsCandidature> DetailsCandidature { get; set; } = new List<DetailsCandidature>();

    public DateTime DateCreation { get; set; }
    public DateTime? DateModification { get; set; }
    public DateTime? DateSuppression { get; set; }
}