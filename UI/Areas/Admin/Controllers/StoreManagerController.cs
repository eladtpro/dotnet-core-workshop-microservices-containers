using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MusicStore.Models;
using MusicStore.Services;

namespace MusicStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize("ManageStore")]
    public class StoreManagerController : Controller
    {
        private readonly ICatalogService _catalogService;

        public StoreManagerController(ICatalogService catalogService)
        {
            _catalogService = catalogService;
        }


        //
        // GET: /StoreManager/
        public async Task<IActionResult> Index()
        {
            var albums = await _catalogService.GetMusic();
            return View(albums);
        }

        //
        // GET: /StoreManager/Details/5
        public async Task<IActionResult> Details(int id)
        {
            AlbumDTO album = await _catalogService.GetMusic(id);

            if (album == null)
                return NotFound();

            return View(album);
        }

        //
        // GET: /StoreManager/Create
        public IActionResult Create()
        {
            var genreTask = _catalogService.GetAllGenres();
            var artistTask = _catalogService.GetAllArtists();
            Task.WaitAll(genreTask, artistTask);

            ViewBag.GenreId = new SelectList(genreTask.Result, "GenreId", "Name");
            ViewBag.ArtistId = new SelectList(artistTask.Result, "ArtistId", "Name");

            return View();
        }

        // POST: /StoreManager/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( AlbumDTO album)
        {
            if (ModelState.IsValid)
            {
                await _catalogService.Create(album);
                return RedirectToAction("Index");
            }

            GetArtistsAndGenres(album);
            return View(album);
        }

        //
        // GET: /StoreManager/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            AlbumDTO album = await _catalogService.GetMusic(id);

            GetArtistsAndGenres(album);

            return View(album);
        }

        //
        // POST: /StoreManager/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AlbumDTO album, CancellationToken requestAborted)
        {
            if (ModelState.IsValid)
            {
                await _catalogService.Update(album);
                return RedirectToAction("Index");
            }
            
            GetArtistsAndGenres(album);

            return View(album);
        }

        //
        // GET: /StoreManager/RemoveAlbum/5
        public async Task<IActionResult> RemoveAlbum(int id)
        {
            AlbumDTO album = await _catalogService.GetMusic(id);

            if (album == null)
                return NotFound();

            return View(album);
        }

        //
        // POST: /StoreManager/RemoveAlbum/5
        [HttpPost]
        [ActionName("RemoveAlbum")]
        public async Task<IActionResult> RemoveAlbumConfirmed(int id, CancellationToken requestAborted)
        {
            AlbumDTO album = await _catalogService.GetMusic(id);
            if (album == null)
                return NotFound();

            //TODO:No Delete Music API Exposed
            return RedirectToAction("Index");
        }

        private void GetArtistsAndGenres(AlbumDTO album)
        {
            //grab lookup data as parallel tasks
            var genreTask = _catalogService.GetAllGenres();
            var artistTask = _catalogService.GetAllArtists();
            Task.WaitAll(genreTask, artistTask);
            
            ViewBag.GenreId = new SelectList(genreTask.Result, "GenreId", "Name", album.GenreId);
            ViewBag.ArtistId = new SelectList(artistTask.Result, "ArtistId", "Name", album.ArtistId);
        }
    }
}