namespace Catalog.API.Domain
{
    public class NewItem
    {
        public int AlbumId { get; set; }
        public int AvailableStock { get; set; }
        public string correlationToken { get; set; }
    }
}