using System;
using System.Runtime.Serialization;

namespace Catalog.API.Application.Dtos
{
    [DataContract]
    public class MusicAddDto
    {
        [DataMember] public int Id { get; set; }
        [DataMember] public string Title { get; set; }

        [DataMember] public string AlbumArtUrl { get; set; }

        [DataMember] public bool ParentalCaution { get; set; }

        [DataMember] public string Upc { get; set; }

        [DataMember] public bool? Cutout { get; set; }

        [DataMember] public DateTime? ReleaseDate { get; set; }

        [DataMember] public decimal Price { get; set; }

        [DataMember] public int ArtistId { get; set; }

        [DataMember] public int GenreId { get; set; }
    }
}