namespace Front_Office_Concours_Admin.Models;


public class AnnoncePagedViewModel
{
    public List<Annonce> Annonces { get; set; } = new();
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }

    public int TotalPages =>
        (int)Math.Ceiling((double)TotalItems / PageSize);
}
