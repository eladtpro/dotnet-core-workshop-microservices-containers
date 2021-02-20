using System.Collections.Generic;
using System.Threading.Tasks;
using MusicStore.Models;

namespace MusicStore.Services
{
    public interface ICatalogService
    {
        Task<List<AlbumDTO>> GetTopSellingMusic(int count);
        Task<AlbumDTO> GetMusic(int id);
        Task<GenreDto> GetGenre(int id, bool includeAlbums = false);
        Task<List<GenreDto>> GetAllGenres(bool includeAlbums = false);
        Task<IEnumerable<AlbumDTO>> GetMusic();
        Task<List<AlbumDTO>> GetAllAlbums();
        Task<List<ArtistDto>> GetAllArtists();
        Task<AlbumDTO> Update(AlbumDTO album);
        Task<AlbumDTO> Create(AlbumDTO album);
    }
}