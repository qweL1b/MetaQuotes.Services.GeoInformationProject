using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetaQuotes.Services.GeoInformationService.Domain.Models;

namespace MetaQuotes.Services.GeoInformationService.Application.Services
{
    public interface IGeoSearching
    {
        Task<LocationRecord?> FindLocationByIpAsync(string ip);

        Task<IEnumerable<LocationRecord>> FindLocationsByCityAsync(string cityName);
    }
}
