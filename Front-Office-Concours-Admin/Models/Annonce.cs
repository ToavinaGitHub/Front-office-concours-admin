namespace Front_Office_Concours_Admin.Models;

public class Annonce
{
    public int Id { get; set; }

    public string Titre { get; set; }
    public string Description { get; set; }

    public int EntiteId { get; set; }
    public Entite? Entite { get; set; }
    public int StatutAnnonceId { get; set; }
    public StatutAnnonce? StatutAnnonce { get; set; }

    public int? TypeContratId { get; set; }
    public TypeContrat? TypeContrat { get; set; }
    public int? TypeEmploiId { get; set; }
    public TypeEmploi? TypeEmploi { get; set; }

    public string LieuPoste { get; set; }

    public DateTime DateLimiteDepotDossier { get; set; }
    public DateTime? DateConcours { get; set; }

    public string? TachesPrincipales { get; set; }

    public DateTime DateCreation { get; set; }
    public DateTime? DateModification { get; set; }
    public DateTime? DateSuppression { get; set; }
}