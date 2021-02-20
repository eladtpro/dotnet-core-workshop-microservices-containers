using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.API.Contracts;
using Catalog.API.Domain;
using Catalog.API.Infrastructure.DataStore;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Infrastructure.Repository
{
    public class MusicRepository : BaseRepository<Product>, IMusicRepository
    {
        public MusicRepository(DataContext ctx) : base(ctx)
        {
        }

        public async Task<int> GetCount(string correlationToken)
        {
            return await Get().CountAsync();
        }

        public async Task<IList<Product>> GetTopSellers(int count, string correlationToken)
        {
            // Group the order details by album and return the albums with the highest count
            return await Get()
                // TODO: Add logic that reads orders to build Top Sellers list 
                //       based upon orders
                //.OrderByDescending(x => x.OrderDetails.Count())
                .Take(count)
                .AsNoTracking() // Disable change tracking
                .Include(x => x.Artist)
                .Include(y => y.Genre)
                .ToListAsync();
        }

        public async Task<List<Product>> GetAll(string correlationToken)
        {
            return await Get().Include(x => x.Artist).Include(y => y.Genre).ToListAsync();
        }

        public async Task<Product> GetById(int id, string correlationToken)
        {
            return await Get().Where(x => x.Id == id).Include(x => x.Artist).Include(y => y.Genre).FirstOrDefaultAsync();
        }

        public async Task<Product> GetByIdWithIdempotencyCheck(int id, string correlationToken)
        {
            return await Get().Where(x => x.Id == id && x.CorrelationId == new Guid(correlationToken)).Include(x => x.Artist).Include(y => y.Genre).FirstOrDefaultAsync();
        }


        public async Task<bool> ChangeParentalCaution(int albumId, bool parentalCaution, string correlationToken)
        {
            Product album = GetById(albumId, correlationToken).Result;

            if (album == null)
                return false;

            // If ParentalCaution has not changed, the short-circuit operation
            // and return true, avoiding expense of unneccesary update.
            if (album.ParentalCaution == parentalCaution)
                return true;

            album.ParentalCaution = parentalCaution;

            await Update(album);

            return true;
        }

        public async Task<IList<Product>> RetrieveArtistsForGenre(int genreId, string correlationToken)
        {
            return await Get()
                .Include("Artists")
                .Where(x => x.GenreId == genreId).ToListAsync();
        }

        public async Task<IList<Product>> GetInexpensiveAlbumsByGenre(int genreId, decimal priceCeiling, string correlationToken)
        {
            throw new NotImplementedException();
            //return await base.Find(x => x.GenreId == genreId && x.Price <= priceCeiling).ToListAsync();
        }
    }
}