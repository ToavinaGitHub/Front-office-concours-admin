namespace Front_Office_Concours_Admin.Models;

public class Entite
{
    public     int? Id { get; set; }
    public     string Nom { get; set; }
    public DateTime? DateCreation { get; set; }
    public DateTime? DateModification { get; set; }
    public DateTime? DateSuppession { get; set; }
}