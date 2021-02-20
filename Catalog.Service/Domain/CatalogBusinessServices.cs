using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Catalog.API.Contracts;
using Catalog.API.Events;
using EventBus.EventBus;
using EventBus.Events;

namespace Catalog.API.Domain
{
    public class CatalogBusinessServices : ICatalogBusinessServices
    {
        private readonly IArtistRepository _artistRepository;
        private readonly IEventBus _eventBus;
        private readonly IGenreRepository _genreRepository;
        private readonly IMusicRepository _musicRepository;

        public CatalogBusinessServices(IMusicRepository musicRepository,
            IGenreRepository genreRepository,
            IArtistRepository artistRepository,
            IEventBus eventBus)
        {
            _musicRepository = musicRepository;
            _genreRepository = genreRepository;
            _artistRepository = artistRepository;
            _eventBus = eventBus;
        }

        public async Task<IList<Product>> GetAllMusic(string correlationToken)
        {
            return await _musicRepository.GetAll(correlationToken);
        }

        public async Task<Product> GetMusic(string correlationToken, int albumId)
        {
            return await _musicRepository.GetById(albumId, correlationToken);
        }

        public async Task<IList<Product>> GetTopSellingMusic(string correlationToken, int count)
        {
            return await _musicRepository.GetTopSellers(count, correlationToken);
        }

        public async Task<IList<Genre>> GetAllGenres(string correlationToken, bool includeAlbums = false)
        {
            return includeAlbums
                ? await _genreRepository.GetAllAndAlbums(correlationToken)
                : await _genreRepository.GetAll(correlationToken);
        }

        public async Task<Genre> GetGenre(int genreId, string correlationToken, bool includeAlbums = false)
        {
            return await _genreRepository.GetById(genreId, correlationToken, includeAlbums);
        }

        public async Task<IList<Artist>> GetAllArtists(string correlationToken)
        {
            return await _artistRepository.GetAll(correlationToken);
        }

        public async Task<Artist> GetArtist(int artistID, string correlationToken)
        {
            return await _artistRepository.GetById(artistID, correlationToken);
        }

        public async Task<Product> Add(string correlationToken, Product product)
        {
            // Idempotent write check. Ensure insert with same correlation token has
            // not already happened. This would most likely do to a retry after the
            // product has been added.
            var targetAlbum = await _musicRepository.GetByIdWithIdempotencyCheck(product.Id, correlationToken);

            if (targetAlbum == null)
            {
                // Product has not been added yet
                 await _musicRepository.Add(product);
            }
            else
            {
                // Assign Id from original insert
                product.Id = targetAlbum.Id;
            }

            //************** Publish Event  *************************
            // Publish event that informs application that product has changed
            //   First Argument: Event Data object
            //   Second Argument: EventType Enum
            //   Third Argrment: Correalation Token
            await _eventBus.Publish(await PrepareProductChangedEvent(product, correlationToken),
                MessageEventEnum.ProductChangedEvent,
                correlationToken);

            return product;
        }

        public async Task<Product> Update(string correlationToken, Product product)
        {
            await _musicRepository.Update(product);

            //************** Publish Event  *************************
            // Publish event that informs application that product has changed
            //   First Argument: Event Data object
            //   Second Argument: EventType Enum
            //   Third Argrment: Correalation Token
            await _eventBus.Publish(await PrepareProductChangedEvent(product, correlationToken),
                MessageEventEnum.ProductChangedEvent,
                correlationToken);

            return product;
        }

        public Task<bool> Remove(string correlationToken, int albumId)
        {
            throw new NotImplementedException();
        }

        private async Task<ProductChangedEvent> PrepareProductChangedEvent(Product product, string correlationToken)
        {
            // Perform Lookup to get Genre and Artist Names
            Artist artist = await _artistRepository.GetById(product.ArtistId, correlationToken);
            Genre genre = await _genreRepository.GetById(product.GenreId, correlationToken);

            // Populate data in the event object
            var productChangedEvent = new ProductChangedEvent
            {
                Id = product.Id,
                Title = product.Title,
                // Provide fallback logic in the event we cannot fetch Artist or Genre name
                ArtistName = artist.Name ?? "Unknown Artist",
                GenreName = genre.Name ?? "Unknown Genre",
                Price = product.Price,
                ReleaseDate = product.ReleaseDate ?? DateTime.Now,
                ParentalCaution = product.ParentalCaution,
                Upc = product.Upc,
                Cutout = product.Cutout
            };

            return productChangedEvent;
        }
    }
}