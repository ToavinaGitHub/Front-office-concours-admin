namespace Front_Office_Concours_Admin.Models;

public class AnnonceElastic
{
    public int? id { get; set; }
    public string? titre { get; set; }
    public string? description { get; set; }
    public string? lieuPoste { get; set; }
    public string? typeContrat { get; set; }
    public string? horaire { get; set; }
    public DateTime? dateCreation { get; set; }
}
