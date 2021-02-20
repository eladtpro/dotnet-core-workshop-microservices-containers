using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.API.Contracts;
using Catalog.API.Domain;
using Catalog.API.Infrastructure.DataStore;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Infrastructure.Repository
{
    public class GenreRepository : BaseRepository<Genre>, IGenreRepository
    {
        public GenreRepository(DataContext ctx) : base(ctx)
        {
        }

        public async Task<Genre> GetById(int id, string correlationToken, bool includeAlbums = false)
        {
            return includeAlbums ? await Get().Include(x => x.Albums).SingleOrDefaultAsync(g => g.GenreId == id) : FindById(id).Result;
        }

        public async Task<List<Genre>> GetAll(string correlationToken)
        {
            return await Get().ToListAsync();
        }

        public async Task<List<Genre>> GetAllAndAlbums(string correlationToken)
        {
            return await Get().Include(x => x.Albums).ToListAsync();
        }

        //public override void Add(Genre genre)
        //{
        //    Update(genre);
        //    // Explicitly call save changes as we will need 
        //    // the new Id to create the Location Header.
        //    // With UnitOfWork, it does not call SaveChanges
        //    // until after the controller action executes.
        //    SaveChanges();
        //}


        public async Task<Genre> GetGenreAndAlbums(string genre, string correlationToken)
        {
            return await Get().Include(x => x.Albums).SingleOrDefaultAsync(x => x.Name == genre);
        }
    }
}