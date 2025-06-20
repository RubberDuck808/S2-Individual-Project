using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace APIWrapper
{
    public class GoogleMapsApiWrapper : IGoogleMapsApiWrapper
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GoogleMapsApiWrapper(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _apiKey = config["GOOGLE_API_KEY"]
                   ?? Environment.GetEnvironmentVariable("GOOGLE_API_KEY")
                   ?? throw new Exception("GOOGLE_API_KEY not found in config or environment variables.");
        }


        public async Task<(double lat, double lng)?> GetCoordinatesFromAddressAsync(string address)
        {
            var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={Uri.EscapeDataString(address)}&key={_apiKey}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return null;

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonDocument.Parse(content);
            var root = result.RootElement;

            if (!root.TryGetProperty("results", out var resultsArray) || resultsArray.GetArrayLength() == 0)
                return null;

            var location = resultsArray[0]
                .GetProperty("geometry")
                .GetProperty("location");

            var lat = location.GetProperty("lat").GetDouble();
            var lng = location.GetProperty("lng").GetDouble();

            return (lat, lng);
        }

        public async Task<(string? City, string? PostCode)> GetCityAndPostalCodeFromAddressAsync(string address)
        {
            var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={Uri.EscapeDataString(address)}&key={_apiKey}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return (null, null);

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonDocument.Parse(content);
            var root = result.RootElement;

            if (!root.TryGetProperty("results", out var resultsArray) || resultsArray.GetArrayLength() == 0)
                return (null, null);

            var addressComponents = resultsArray[0].GetProperty("address_components");
            string? city = null;
            string? postcode = null;

            foreach (var component in addressComponents.EnumerateArray())
            {
                if (component.TryGetProperty("types", out var types))
                {
                    foreach (var type in types.EnumerateArray())
                    {
                        if (type.GetString() == "locality")
                            city = component.GetProperty("long_name").GetString();
                        else if (type.GetString() == "postal_code")
                            postcode = component.GetProperty("long_name").GetString();
                    }
                }
            }

            return (city, postcode);
        }
    }
}
