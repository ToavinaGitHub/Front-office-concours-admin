namespace Front_Office_Concours_Admin.Models;

public class Candidature
{
    public int Id { get; set; }

    public int AnnonceId { get; set; }
    public Annonce Annonce { get; set; }

    public int StatutCandidatureId { get; set; }
    public StatutCandidature StatutCandidature { get; set; }

    public int CandidatId { get; set; }
    public Candidat Candidat { get; set; }

    public string CV { get; set; }
    public string LettreMotivation { get; set; }

    public DateTime DateCreation { get; set; }
    public DateTime? DateModification { get; set; }
    public DateTime? DateSuppression { get; set; }

    public List<DetailsCandidature> DetailsCandidature { get; set; } = new List<DetailsCandidature>();
}