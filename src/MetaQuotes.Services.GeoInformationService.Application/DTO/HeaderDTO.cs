using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaQuotes.Services.GeoInformationService.Domain.Models
{
    public class HeaderDTO
    {
        public int Version { get; set; }
        public string Name { get; set; }
        public ulong Timestamp { get; set; }
        public int Records { get; set; }
        public uint OffsetRanges { get; set; }
        public uint OffsetCities { get; set; }
        public uint OffsetLocations { get; set; }
    }
}
