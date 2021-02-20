using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MusicStore.Services;

namespace MusicStore.Components
{
    [ViewComponent(Name = "GenreMenu")]
    public class GenreMenuComponent : ViewComponent
    {
        private readonly ICatalogService _catalogService;

        public GenreMenuComponent(ICatalogService catalogService)
        {
            _catalogService = catalogService;
        }


        public async Task<IViewComponentResult> InvokeAsync()
        {
            var genres = await _catalogService.GetAllGenres();

            return View(genres);
        }
    }
}