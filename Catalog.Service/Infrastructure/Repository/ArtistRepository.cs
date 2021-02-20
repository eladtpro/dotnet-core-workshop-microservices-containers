using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.API.Contracts;
using Catalog.API.Domain;
using Catalog.API.Infrastructure.DataStore;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Infrastructure.Repository
{
    public class ArtistRepository : BaseRepository<Artist>, IArtistRepository
    {
        public ArtistRepository(DataContext ctx) : base(ctx)
        {
        }

        public async Task<Artist> GetById(int id, string correlationToken)
        {
            return await FindById(id);
        }

        public async Task<IList<Artist>> GetAll(string correlationToken)
        {
            return await Get().ToListAsync();
        }
    }
}