namespace Front_Office_Concours_Admin.Models;

public class InscriptionViewModel
{
    public string Nom { get; set; }
    public string Prenom { get; set; }
    public DateTime? DateNaissance { get; set; }
    public string Adresse { get; set; }
    public string Telephone { get; set; }
    public string Genre { get; set; }
    public string Email { get; set; }

    public string MotDePasse { get; set; }
    public string ConfirmMotDePasse { get; set; }
}