using System.Collections.Generic;
using System.Threading.Tasks;
using Catalog.API.Domain;

namespace Catalog.API.Contracts
{
    public interface ICatalogBusinessServices
    {
        Task<IList<Product>> GetAllMusic(string correlationToken);
        Task<Product> GetMusic(string correlationToken, int albumId);
        Task<IList<Product>> GetTopSellingMusic(string correlationToken, int count);
        Task<IList<Genre>> GetAllGenres(string correlationToken, bool includeAlbums = false);
        Task<IList<Artist>> GetAllArtists(string correlationToken);
        Task<Artist> GetArtist(int artistID, string correlationToken);
        Task<Product> Add(string correlationToken, Product product);
        Task<Product> Update(string correlationToken, Product product);
        Task<bool> Remove(string correlationToken, int albumId);
        Task<Genre> GetGenre(int genreId, string correlationToken, bool includeAlbums = false);
    }
}