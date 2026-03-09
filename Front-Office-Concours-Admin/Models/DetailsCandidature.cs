using System.ComponentModel.DataAnnotations.Schema;

namespace Front_Office_Concours_Admin.Models;

public class DetailsCandidature
{
    public int Id { get; set; }

    public int CandidatureId { get; set; }
    public Candidature Candidature { get; set; }

    public int ExigenceId { get; set; }
    public Exigence Exigence { get; set; }

    public bool Valeur { get; set; }
    public string PieceJustificative { get; set; }
    
    [NotMapped]
    public IFormFile PieceFile { get; set; }


    public DateTime DateCreation { get; set; }
    public DateTime? DateModification { get; set; }
    public DateTime? DateSuppression { get; set; }
}