using System.Collections.Generic;
using System.Threading.Tasks;
using MusicStore.Helper;
using MusicStore.Models;

namespace MusicStore.Services
{
    public class CatalogService : ICatalogService
    {
        private const string baseUrl = "api/CatalogServices";
        private readonly IRestClient _restClient;

        public CatalogService(IRestClient restClient)
        {
            _restClient = restClient;
        }

        public async Task<List<AlbumDTO>> GetTopSellingMusic(int count = 10)
        {
            var result = await _restClient.GetAsync<List<AlbumDTO>>($"{baseUrl}/TopSellingMusic/{count}");
            return result.Data;
        }

        public async Task<AlbumDTO> GetMusic(int id)
        {
            var result = await _restClient.GetAsync<AlbumDTO>($"{baseUrl}/Music/{id} ");
            return result.Data;
        }

        public async Task<IEnumerable<AlbumDTO>> GetMusic()
        {
            var result = await _restClient.GetAsync<List<AlbumDTO>>($"{baseUrl}/Music");
            return result.Data;
        }

        public async Task<GenreDto> GetGenre(int id, bool includeAlbums = false)
        {
            var result = await _restClient.GetAsync<GenreDto>($"{baseUrl}/Genre/{id}?includeAlbums={includeAlbums}");
            return result.Data;
        }

        public async Task<List<GenreDto>> GetAllGenres(bool includeAlbums = false)
        {
            var result = await _restClient.GetAsync<List<GenreDto>>($"{baseUrl}/Genres/?includeAlbums={includeAlbums}");
            return result.Data;
        }

        public async Task<List<AlbumDTO>> GetAllAlbums()
        {
            var result = await _restClient.GetAsync<List<AlbumDTO>>($"{baseUrl}/Music/");
            return result.Data;
        }

        public async Task<List<ArtistDto>> GetAllArtists()
        {
            var result = await _restClient.GetAsync<List<ArtistDto>>($"{baseUrl}/Artists/");
            return result.Data;
        }

        public async Task<AlbumDTO> Update(AlbumDTO album)
        {
            var result = await _restClient.PutAsync<AlbumDTO>($"{baseUrl}/Music", album);
            return result.Data;
        }
        public async Task<AlbumDTO> Create(AlbumDTO album)
        {
            var result = await _restClient.PostAsync<AlbumDTO>($"{baseUrl}/Music", album);
            return result.Data;
        }
    }
}