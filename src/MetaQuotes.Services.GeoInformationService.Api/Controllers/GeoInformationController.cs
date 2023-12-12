using MetaQuotes.Services.GeoInformationService.Domain.Models;
using MetaQuotes.Services.GeoInformationService.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace MetaQuotes.Services.GeoInformationService.UI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GeoInformationController : ControllerBase
    {
        private readonly ILogger<GeoInformationController> _logger;

        private readonly GeoBaseService _geoBaseService;

        public GeoInformationController(ILogger<GeoInformationController> logger, GeoBaseService geoBaseService)
        {
            _logger = logger;
            _geoBaseService = geoBaseService;
        }

        [HttpGet("/ip/location")]
        public ActionResult<LocationRecordDTO?> GetLocationByIp(string ip)
        {
            var result = _geoBaseService.FindLocationByIp(ip);

            if (result == null) return NotFound(null);

            return Ok(result);
        }

        [HttpGet("/city/locations")]
        public ActionResult<IEnumerable<LocationRecordDTO>> GetLocationsByCity(string city)
        {
            var result = _geoBaseService.FindLocationsByCity(city);

            if(result == null) return NotFound(null);

            return Ok(result);
        }
    }
}