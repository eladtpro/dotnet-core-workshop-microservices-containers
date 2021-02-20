using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.API.Application.Dtos;
using Catalog.API.Contracts;
using Catalog.API.Domain;
using Microsoft.AspNetCore.Mvc;
using RestCommunication;
using Utilities;

namespace Catalog.API.Controllers
{
    /// <summary>
    ///     Microservice that manages product catalog items
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CatalogController : ControllerBase
    {
        private const int _topSellingCount = 5;

        private readonly ICatalogBusinessServices _catalogBusinessServices;

        public CatalogController(ICatalogBusinessServices catalogBusinessServices)
        {
            _catalogBusinessServices = catalogBusinessServices;
        }

        /// <summary>
        ///     Gets All Music
        /// </summary>
        /// <param name="correlationToken">Tracks request - Can be any value</param>
        /// <returns>All Music Products</returns>
        [ProducesResponseType(typeof(List<Product>), 200)]
        [HttpGet("Music/{correlationToken}", Name = "GetAllMusicRoute")]
        public async Task<IActionResult> GetAllMusic(string correlationToken)
        {
            Guard.ForNullOrEmpty(correlationToken, "correlationToken");

            var albums = await _catalogBusinessServices.GetAllMusic(correlationToken);

            if (albums.Count < 1)
                return new ObjectResult(new List<MusicDto>());

            // ObjectResult return type is capable of content negotiation
            return new ObjectResult(Mapper.MapToMusicDto(albums));
        }

        /// <summary>
        ///     Get specific music item
        /// </summary>
        /// <param name="correlationToken">Tracks request - Can be any value</param>
        /// <param name="id">Id of music -- cannot be zero or negative</param>
        /// <returns>Specific Music Product</returns>
        [ProducesResponseType(typeof(Product), 200)]
        [HttpGet("Music/{correlationToken}/{id}", Name = "GetMusicRoute")]
        public async Task<IActionResult> GetMusic(string correlationToken, int id)
        {
            Guard.ForNullOrEmpty(correlationToken, "correlationToken");
            Guard.ForLessEqualZero(id, "albumId");

            var album = await _catalogBusinessServices.GetMusic(correlationToken, id);

            if (album == null)
                return new ObjectResult(album);

            return new ObjectResult(Mapper.MapToMusicDto(album));
        }

        /// <summary>
        ///     Get top-selling music items
        /// </summary>
        /// <param name="correlationToken">Tracks request - Can be any value</param>
        /// <param name="count">Number of items to return</param>
        /// <returns>List of Top Selling Items</returns>
        [ProducesResponseType(typeof(Product), 200)]
        [HttpGet("TopSellingMusic/{correlationToken}/{count}", Name = "GetTopSellingMusicRoute")]
        public async Task<IActionResult> GetTopSellingMusic(string correlationToken, int count = _topSellingCount)
        {
            Guard.ForNullOrEmpty(correlationToken, "correlationToken");
            Guard.ForLessEqualZero(count, "count");

            var albums = await _catalogBusinessServices.GetTopSellingMusic(correlationToken, count);

            if (albums.Count < 1)
                return new ObjectResult(new List<MusicDto>());

            return new ObjectResult(Mapper.MapToMusicDto(albums));
        }

        /// <summary>
        ///     Gets All Genres
        /// </summary>
        /// <param name="correlationToken">Tracks request - Can be any value</param>
        /// <param name="includeAlbums">Optionally include Albums associated with each genre</param>
        /// <returns>List of all Genre Types</returns>
        [ProducesResponseType(typeof(List<GenreDto>), 200)]
        [HttpGet("Genres/{correlationToken}", Name = "GetAllGenreRoute")]
        public async Task<IActionResult> GetAllGenres(string correlationToken, [FromQuery] bool includeAlbums)
        {
            Guard.ForNullOrEmpty(correlationToken, "correlationToken");

            var genres = await _catalogBusinessServices.GetAllGenres(correlationToken, includeAlbums);

            if (genres.Count < 1)
                return new ObjectResult(new List<GenreDto>());

            // ObjectResult return type is capable of content negotiation
            return new ObjectResult(Mapper.MapToGenreDto(genres));
        }

        /// <summary>
        ///     Get specific Genre for specified Id
        /// </summary>
        /// <param name="correlationToken">Tracks request - Can be any value</param>
        /// <param name="id">Id of music -- cannot be zero or negative</param>
        /// <returns>Specific Genre Type</returns>
        [ProducesResponseType(typeof(GenreDto), 200)]
        [HttpGet("Genre/{correlationToken}/{id}", Name = "GetGenreRoute")]
        public async Task<IActionResult> GetGenre(string correlationToken, int id, [FromQuery] bool includeAlbums)
        {
            Guard.ForNullOrEmpty(correlationToken, "correlationToken");
            Guard.ForLessEqualZero(id, "GenreId");

            var genre = await _catalogBusinessServices.GetGenre(id, correlationToken, includeAlbums);

            if (genre == null)
                return new ObjectResult(genre);

            return new ObjectResult(Mapper.MapToGenreDto(genre));
        }

        /// <summary>
        ///     Gets All Artists
        /// </summary>
        /// <param name="correlationToken">Tracks request - Can be any value</param>
        /// <returns>List of all Artist Types</returns>
        [ProducesResponseType(typeof(List<ArtistDto>), 200)]
        [HttpGet("Artists/{correlationToken}", Name = "GetAllArtistsRoute")]
        public async Task<IActionResult> GetAllArtists(string correlationToken)
        {
            Guard.ForNullOrEmpty(correlationToken, "correlationToken");

            var artists = await _catalogBusinessServices.GetAllArtists(correlationToken);

            if (artists.Count < 1)
                return new ObjectResult(new List<ArtistDto>());

            // ObjectResult return type is capable of content negotiation
            return new ObjectResult(Mapper.MapToArtistDto(artists));
        }

        /// <summary>
        ///     Adds new music item
        /// </summary>
        /// <param name="musicDto">Required music information - Id must be ZERO value</param>
        /// <param name="correlationToken">Tracks request - Can be any value</param>
        /// <returns></returns>
        // POST: api/Catalog
        [ProducesResponseType(typeof(MusicDto), 201)]
        [HttpPost(Name = "PostMusicRoute")]
        public async Task<IActionResult> Post([FromBody] MusicAddDto musicDto, string correlationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Guard.ForNullOrEmpty(correlationToken, "correlationToken");
            Guard.ForNullObject(musicDto, "musicDto class is missing");

            var newAlbum =   Mapper.MapToMusicDtoPost(musicDto);

            await _catalogBusinessServices.Add(correlationToken, newAlbum);

            var album = Mapper.MapToMusicDto(newAlbum);

            return CreatedAtRoute("PostMusicRoute", album);
        }

        /// <summary>
        ///     Updates existing music item
        /// </summary>
        /// <param name="musicUpdateDto">Required music information - Id must be non-ZERO value</param>
        /// <param name="correlationToken">Tracks request - Can be any value</param>
        /// <returns></returns>
        // PUT: api/Catalog/5
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] MusicUpdateDto musicUpdateDto, string correlationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Guard.ForNullOrEmpty(correlationToken, "correlationToken");
            Guard.ForNullObject(musicUpdateDto, "Album");
            Guard.ForLessEqualZero(musicUpdateDto.Id, "album.Id");

            var updatedAlbum = Mapper.MapDtoToProduct(musicUpdateDto);

            updatedAlbum = await _catalogBusinessServices.Update(correlationToken, updatedAlbum);

            var album = Mapper.MapToMusicDto(updatedAlbum);

            return Ok(album);
        }

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="correlationToken">Tracks request - Can be any value</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, string correlationToken)
        {
            throw new NotImplementedException();

            //Guard.ForNullOrEmpty(correlationToken, "correlationToken");
            //Guard.ForLessEqualZero(id, "albumId");
        }
    }
}