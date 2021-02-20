using System.Collections.Generic;
using System.Threading.Tasks;
using Catalog.API.Domain;

namespace Catalog.API.Contracts
{
    public interface IMusicRepository : IRepository<Product>
    {
        Task<int> GetCount(string correlationToken);
        Task<IList<Product>> GetTopSellers(int count, string correlationToken);
        Task<List<Product>> GetAll(string correlationToken);
        Task<Product> GetById(int id, string correlationToken);
        Task<bool> ChangeParentalCaution(int albumId, bool parentalCaution, string correlationToken);
        Task<IList<Product>> RetrieveArtistsForGenre(int genreId, string correlationToken);
        Task<IList<Product>> GetInexpensiveAlbumsByGenre(int genreId, decimal priceCeiling, string correlationToken);
        Task<Product> GetByIdWithIdempotencyCheck(int id, string correlationToken);
    }
}