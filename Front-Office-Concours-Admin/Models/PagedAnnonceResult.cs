
namespace Front_Office_Concours_Admin.Models
{
    public class PagedAnnonceResult
    {
        public List<AnnonceElasticSearchResult> Annonces { get; set; } = new List<AnnonceElasticSearchResult>();
        public int Total { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }
}