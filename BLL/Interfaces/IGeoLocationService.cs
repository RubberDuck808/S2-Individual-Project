using BLL.DTOs.Accommodation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IGeoLocationService
    {
        Task<(double lat, double lng)?> GetCoordinatesFromAddressAsync(string address);

        Task<(double lat, double lng)?> GetCoordinatesFromAccommodationAsync(AccommodationDto accommodation);

        Task<(string? City, string? PostCode)> GetCityAndPostalCodeFromAddressAsync(string address);

        double CalculateDistanceKm(double lat1, double lon1, double lat2, double lon2);


    }

}
