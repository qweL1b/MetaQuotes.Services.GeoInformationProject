using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaQuotes.Services.GeoInformationService.Domain.Models
{
    public class IpRangeRecordDTO
    {
        public uint IpFrom { get; set; }
        public uint IpTo { get; set; }
        public uint LocationIndex { get; set; }
    }
}
