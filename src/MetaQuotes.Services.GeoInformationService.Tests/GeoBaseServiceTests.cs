using MetaQuotes.Services.GeoInformationService.Infrastructure.Services;
using Xunit;

namespace MetaQuotes.Services.GeoInformationService.Tests
{
    public class GeoBaseServiceTests
    {
        public GeoBaseServiceTests()
        {
        }

        [Theory]
        [InlineData("192.168.0.1", true)] 
        [InlineData("255.255.255.255", false)]
        public async Task LoadData_ShouldCorrectlyLoadData(string ip, bool expectedResult)
        {
            var service = new GeoBaseService();
            service.LoadData("geobase.dat");

            var result = await service.FindLocationByIpAsync(ip);

            Assert.Equal(expectedResult, result != null);
        }
    }
}
