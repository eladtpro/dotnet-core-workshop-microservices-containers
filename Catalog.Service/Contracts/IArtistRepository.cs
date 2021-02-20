using System.Collections.Generic;
using System.Threading.Tasks;
using Catalog.API.Domain;

namespace Catalog.API.Contracts
{
    public interface IArtistRepository : IRepository<Artist>
    {
        Task<IList<Artist>> GetAll(string correlationToken);
        Task<Artist> GetById(int id, string correlationToken);
    }
}