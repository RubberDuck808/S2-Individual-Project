using System.Threading.Tasks;
using APIWrapper;
using BLL.DTOs.Accommodation;
using BLL.Services;
using Moq;
using Xunit;

namespace Tests
{
    public class GeoLocationServiceTests
    {
        private readonly Mock<IGoogleMapsApiWrapper> _mockWrapper;
        private readonly GeoLocationService _service;

        public GeoLocationServiceTests()
        {
            _mockWrapper = new Mock<IGoogleMapsApiWrapper>();
            _service = new GeoLocationService(_mockWrapper.Object);
        }

        // 1. Tests Basic success case for full address → coordinates
        [Fact]
        public async Task GetCoordinatesFromAccommodationAsync_ReturnsCoordinates()
        {
            var acc = new AccommodationDto
            {
                Address = "123 Main St",
                PostCode = "12345",
                City = "Testville",
                Country = "Testland"
            };

            _mockWrapper
                .Setup(m => m.GetCoordinatesFromAddressAsync(It.IsAny<string>()))
                .ReturnsAsync((52.1, 4.3));

            var result = await _service.GetCoordinatesFromAccommodationAsync(acc);

            Assert.NotNull(result);
            Assert.Equal(52.1, result?.lat);
            Assert.Equal(4.3, result?.lng);

            _mockWrapper.Verify(m => m.GetCoordinatesFromAddressAsync(It.IsAny<string>()), Times.Once);
        }

        // 2. Tests Full address → city and postcode
        [Fact]
        public async Task GetCityAndPostalCodeFromAddressAsync_ReturnsExpectedValues()
        {
            _mockWrapper
                .Setup(m => m.GetCityAndPostalCodeFromAddressAsync("Some address"))
                .ReturnsAsync(("Amsterdam", "1000AA"));

            var result = await _service.GetCityAndPostalCodeFromAddressAsync("Some address");

            Assert.Equal("Amsterdam", result.City);
            Assert.Equal("1000AA", result.PostCode);

            _mockWrapper.Verify(m => m.GetCityAndPostalCodeFromAddressAsync("Some address"), Times.Once);
        }

        // 3. Tests Distance calculation between two known cities
        [Fact]
        public void CalculateDistanceKm_ReturnsCorrectDistance()
        {
            double lat1 = 52.3676, lon1 = 4.9041;   // Amsterdam
            double lat2 = 51.9244, lon2 = 4.4777;   // Rotterdam

            double distance = _service.CalculateDistanceKm(lat1, lon1, lat2, lon2);

            Assert.InRange(distance, 55, 70); 
        }

        // 4. Tests Geocoding failure → null return
        [Fact]
        public async Task GetCoordinatesFromAddressAsync_ReturnsNull_WhenApiFails()
        {
            _mockWrapper
                .Setup(m => m.GetCoordinatesFromAddressAsync(It.IsAny<string>()))
                .ReturnsAsync(((double, double)?)null);

            var result = await _service.GetCoordinatesFromAddressAsync("Invalid address");

            Assert.Null(result);
            _mockWrapper.Verify(m => m.GetCoordinatesFromAddressAsync("Invalid address"), Times.Once);
        }

        // 5. Tests - Edge Case - empty address string
        [Fact]
        public async Task GetCoordinatesFromAddressAsync_WithEmptyString_ReturnsNull()
        {
            _mockWrapper
                .Setup(m => m.GetCoordinatesFromAddressAsync(""))
                .ReturnsAsync(((double, double)?)null);

            var result = await _service.GetCoordinatesFromAddressAsync("");

            Assert.Null(result);
            _mockWrapper.Verify(m => m.GetCoordinatesFromAddressAsync(""), Times.Once);
        }

        // 6. Tests - Edge Case - null city or postcode from API
        [Fact]
        public async Task GetCityAndPostalCodeFromAddressAsync_ReturnsNulls_WhenNotFound()
        {
            _mockWrapper
                .Setup(m => m.GetCityAndPostalCodeFromAddressAsync(It.IsAny<string>()))
                .ReturnsAsync((null, null));

            var result = await _service.GetCityAndPostalCodeFromAddressAsync("Unknown location");

            Assert.Null(result.City);
            Assert.Null(result.PostCode);
            _mockWrapper.Verify(m => m.GetCityAndPostalCodeFromAddressAsync("Unknown location"), Times.Once);
        }

        // 7. Tests - Edge Case - completely null accommodation values
        [Fact]
        public async Task GetCoordinatesFromAccommodationAsync_WithEmptyAccommodation_StillCallsApi()
        {
            var acc = new AccommodationDto
            {
                Address = "",
                PostCode = "",
                City = "",
                Country = ""
            };

            _mockWrapper
                .Setup(m => m.GetCoordinatesFromAddressAsync(It.IsAny<string>()))
                .ReturnsAsync((1.23, 4.56));

            var result = await _service.GetCoordinatesFromAccommodationAsync(acc);

            Assert.NotNull(result);
            Assert.Equal(1.23, result?.lat);
            Assert.Equal(4.56, result?.lng);
            _mockWrapper.Verify(m => m.GetCoordinatesFromAddressAsync(It.IsAny<string>()), Times.Once);
        }
    }
}
