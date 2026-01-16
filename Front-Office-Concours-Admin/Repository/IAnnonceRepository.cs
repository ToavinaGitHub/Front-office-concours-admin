using Front_Office_Concours_Admin.Models;

namespace Front_Office_Concours_Admin.Repository;

public interface IAnnonceRepository
{
    AnnoncePagedViewModel GetPagedAnnonces(
        int currentPage,
        int pageSize);
    
    AnnoncePagedViewModel GetPagedAnnoncesByIds (
        List<int> ids,int currentPage,int pageSize);

    DetailsCandidatureResponse GetDetailsCandidatureById(int id);
    
    DetailsAnnonceResponse GetDetailsAnnonceById(int id);
}