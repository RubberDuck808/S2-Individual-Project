using System.Threading.Tasks;

namespace APIWrapper
{
    public interface IGoogleMapsApiWrapper
    {
        Task<(double lat, double lng)?> GetCoordinatesFromAddressAsync(string address);
        Task<(string? City, string? PostCode)> GetCityAndPostalCodeFromAddressAsync(string address);
    }
}
