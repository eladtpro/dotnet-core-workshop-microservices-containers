using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Catalog.API.Application.Dtos
{
    [DataContract]
    public class GenreDto
    {
        [DataMember] public int GenreId { get; set; }

        [DataMember] public string Name { get; set; }

        [DataMember] public string Description { get; set; }

        [DataMember] public List<MusicDto> Albums { get; set; }
    }
}