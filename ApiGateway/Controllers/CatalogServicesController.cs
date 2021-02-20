using System.Collections.Generic;
using System.Threading.Tasks;
using ApiGateway.API.Dtos.Catalog;
using ApiGateway.API.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RestCommunication;
using ServiceDiscovery;

namespace ApiGateway.API.Controllers
{
    // install-package Microsoft.AspNet.WebApi.Client
    //https://stackoverflow.com/questions/10399324/where-is-httpcontent-readasasync

    /// <summary>
    ///     Gateway microservice that manages Product Catalog experience
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CatalogServicesController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IRestClient _restClient;

        public CatalogServicesController(IRestClient restClient, ILogger<CatalogServicesController> logger)
        {
            _restClient = restClient;
            _logger = logger;
        }

        /// <summary>
        ///     Gets All Music Products
        /// </summary>
        /// <returns>All Music Items</returns>
        [ProducesResponseType(typeof(MusicDto), 200)]
        [HttpGet("Music", Name = "GetAllMusicGatewayRoute")]
        public async Task<IActionResult> GetAllMusic()
        {
            List<MusicDto> music;

            // Generate correlationToken
            var correlationToken = CorrelationTokenManager.GenerateToken();

            _logger.LogInformation(LoggingEvents.GetAllMusic, $"Gateway: Getting All Music for {correlationToken}");
            _logger.LogInformation("Fetching all music for {token}", correlationToken);

            // Get music
            //TODO: Add ex handling
            music = await _restClient.GetAsync<List<MusicDto>>(ServiceEnum.Catalog,
                $"api/Catalog/Music/{correlationToken}");

            if (music == null || music.Count < 1)
            {
                _logger.LogWarning(LoggingEvents.GetAllMusic, $"Gateway: No music found for {correlationToken}");
                return BadRequest("No Products were found");
            }

            _logger.LogInformation(LoggingEvents.GetAllMusic, $"Gateway: Retrieved All Music for {correlationToken}");

            return new ObjectResult(music);
        }

        /// <summary>
        ///     Gets Specified Music Product
        /// </summary>
        /// <param name="id">Identifier of Music Item</param>
        /// <returns>Single Music Item</returns>
        [ProducesResponseType(typeof(MusicDto), 200)]
        [HttpGet("Music/{id}", Name = "GetMusicGatewayRoute")]
        public async Task<IActionResult> GetMusic(int id)
        {
            // Generate correlationToken
            var correlationToken = CorrelationTokenManager.GenerateToken();

            // Get music
            //TODO: Add ex handling
            var music = await _restClient.GetAsync<MusicDto>(ServiceEnum.Catalog,
                $"api/Catalog/Music/{correlationToken}/{id}");

            if (music == null)
                return BadRequest($"Product with Id {id} does not exist");

            return new ObjectResult(music);
        }


        /// <summary>
        ///     Gets Top Selling Music Products
        /// </summary>
        /// <param name="count">Items to Show</param>
        /// <returns>Top Selling Music Items</returns>
        [ProducesResponseType(typeof(List<MusicDto>), 200)]
        [HttpGet("TopSellingMusic/{count}", Name = "GetTopSellingMusicGatewayRoute")]
        public async Task<IActionResult> GetTopSellingItems(int count)
        {
            List<MusicDto> music;

            // Generate correlationToken
            var correlationToken = CorrelationTokenManager.GenerateToken();

            // Get music
            //TODO: Add ex handling
            music = await _restClient.GetAsync<List<MusicDto>>(ServiceEnum.Catalog,
                $"api/Catalog/TopSellingMusic/{correlationToken}/{count}");

            if (music == null || music.Count < 1)
                return BadRequest("No Products were found");

            return new ObjectResult(music);
        }

        /// <summary>
        ///     Get Specified Music Genre Type
        /// </summary>
        /// <param name="id">Identifier of Genre Item</param>
        /// <returns>Specific Genre Type</returns>
        [ProducesResponseType(typeof(GenreDto), 200)]
        [HttpGet("Genre/{id:int}", Name = "GetGenreGatewayRoute")]
        public async Task<IActionResult> GetGenre(int id, [FromQuery] bool includeAlbums)
        {
            // Generate correlationToken
            var correlationToken = CorrelationTokenManager.GenerateToken();

            // Get music
            //TODO: Add ex handling
            var genre = await _restClient.GetAsync<GenreDto>(ServiceEnum.Catalog,
                $"api/Catalog/Genre/{correlationToken}/{id}?includeAlbums={includeAlbums}");

            if (genre == null)
                return BadRequest($"Genres with Id {id} could not be found");

            return new ObjectResult(genre);
        }

        /// <summary>
        ///     Gets All Music Genre Types
        /// </summary>
        /// <returns>List of all Genre Types</returns>
        [ProducesResponseType(typeof(List<GenreDto>), 200)]
        [HttpGet("Genres", Name = "GetAllGenresGatewayRoute")]
        public async Task<IActionResult> GetAllGenres([FromQuery] bool includeAlbums)
        {
            List<GenreDto> genres;

            // Generate correlationToken
            var correlationToken = CorrelationTokenManager.GenerateToken();

            // Get music
            //TODO: Add ex handling
            genres = await _restClient.GetAsync<List<GenreDto>>(ServiceEnum.Catalog,
                $"api/Catalog/Genres/{correlationToken}?includeAlbums={includeAlbums}");

            if (genres == null || genres.Count < 1)
                return BadRequest("No Genres were found");

            return new ObjectResult(genres);
        }

        /// <summary>
        ///     Gets All Music Artists
        /// </summary>
        /// <returns>List of all Artist Types</returns>
        [ProducesResponseType(typeof(List<ArtistDto>), 200)]
        [HttpGet("Artists", Name = "GetAllArtistsGatewayRoute")]
        public async Task<IActionResult> GetAllArtists()
        {
            List<ArtistDto> artists;

            // Generate correlationToken
            var correlationToken = CorrelationTokenManager.GenerateToken();

            // Get music
            //TODO: Add ex handling
            artists = await _restClient.GetAsync<List<ArtistDto>>(ServiceEnum.Catalog,
                $"api/Catalog/Artists/{correlationToken}");

            if (artists == null || artists.Count < 1)
                return BadRequest("No Artists were found");

            return new ObjectResult(artists);
        }

        /// <summary>
        ///     Adds New Music Product
        /// </summary>
        /// <param name="musicDto">New Music Item</param>
        // POST: api/ProductCatalogServices
        //[HttpPost(Name = "AddNewProduct")]
        [HttpPost("Music", Name = "AddMusicProductRoute")]
        public async Task<IActionResult> Post([FromBody] MusicDto musicDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Generate correlationToken
            var correlationToken = CorrelationTokenManager.GenerateToken();

            var updatedAlbum = await _restClient.PostAsync<MusicDto>(ServiceEnum.Catalog,
                $"api/Catalog/?correlationToken={correlationToken}", musicDto);

            return new ObjectResult(updatedAlbum);
        }

        /// <summary>
        ///     Updates Existing Music Product
        /// </summary>
        /// <param name="musicDto">New Music Item</param>
        // POST: api/ProductCatalogServices
        [HttpPut("Music", Name = "UpdateMusicProductRoute")]
        public async Task<IActionResult> Put([FromBody] MusicDtoUpdate musicDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Generate correlationToken
            var correlationToken = CorrelationTokenManager.GenerateToken();

            var updatedAlbum = await _restClient.PutAsync<MusicDtoUpdate>(ServiceEnum.Catalog,
                $"api/Catalog/?correlationToken={correlationToken}", musicDto);

            return new ObjectResult(updatedAlbum);
        }
    }
}