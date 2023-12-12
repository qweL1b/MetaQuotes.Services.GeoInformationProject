using MetaQuotes.Services.GeoInformationService.Application.Services;
using MetaQuotes.Services.GeoInformationService.Domain.Models;
using MetaQuotes.Services.GeoInformationService.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace MetaQuotes.Services.GeoInformationService.UI.Controllers
{
    [ApiController]
    public class GeoInformationController : ControllerBase
    {
        private readonly ILogger<GeoInformationController> _logger;

        private readonly IGeoSearching _geoBaseService;

        public GeoInformationController(ILogger<GeoInformationController> logger, IGeoSearching geoBaseService)
        {
            _logger = logger;
            _geoBaseService = geoBaseService;
        }

        [HttpGet("/ip/location")]
        public async Task<ActionResult<LocationRecordDTO?>> GetLocationByIp(string ip)
        {
            var result = await _geoBaseService.FindLocationByIpAsync(ip);

            if (result == null) return NotFound();

            return Ok(result);
        }

        [HttpGet("/city/locations")]
        public async Task<ActionResult<IEnumerable<LocationRecordDTO>>> GetLocationsByCity(string city)
        {
            var result = await _geoBaseService.FindLocationsByCityAsync(city);

            if(result == null) return NotFound();

            return Ok(result);
        }
    }
}