using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MusicStore.Models;
using MusicStore.Services;

namespace MusicStore.Controllers
{
    public class StoreController : Controller
    {
        private readonly ICatalogService _catalogService;

        public StoreController(ICatalogService catalogService)
        {
            _catalogService = catalogService;
        }


        //
        // GET: /Store/
        public async Task<IActionResult> Index()
        {
            var genres = await _catalogService.GetAllGenres();

            return View(genres);
        }

        //
        // GET: /Store/Browse?genre=Disco
        public async Task<IActionResult> Browse(int id)
        {
            // Retrieve Genre genre and its Associated associated Albums albums from database
            GenreDto genreModel = await _catalogService.GetGenre(id, true);


            if (genreModel == null)
                return NotFound();

            return View(genreModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            AlbumDTO album = await _catalogService.GetMusic(id);


            if (album == null)
                return NotFound();

            return View(album);
        }
    }
}