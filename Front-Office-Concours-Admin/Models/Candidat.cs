namespace Front_Office_Concours_Admin.Models;

public class Candidat
{
    public int Id { get; set; }

    public string Prenom { get; set; }
    public string Nom { get; set; }
    public DateOnly DateNaissance { get; set; }
    public string Adresse { get; set; }
    public string Telephone { get; set; }
    public string Email { get; set; }
    public string MotDePasse { get; set; }
    public string Genre { get; set; }

    public DateTime DateCreation { get; set; }
    public DateTime? DateModification { get; set; }
    public DateTime? DateSuppression { get; set; }
}