using APIWrapper;
using BLL.DTOs.Accommodation;
using BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class GeoLocationService : IGeoLocationService
    {
        private readonly GoogleMapsApiWrapper _mapsApi;

        public GeoLocationService(GoogleMapsApiWrapper mapsApi)
        {
            _mapsApi = mapsApi;
        }

        public async Task<(double lat, double lng)?> GetCoordinatesFromAccommodationAsync(AccommodationDto acc)
        {
            var address = $"{acc.Address}, {acc.PostCode} {acc.City}, {acc.Country}";
            return await _mapsApi.GetCoordinatesFromAddressAsync(address);
        }

        public async Task<(double lat, double lng)?> GetCoordinatesFromAddressAsync(string address)
        {
            var coords = await _mapsApi.GetCoordinatesFromAddressAsync(address);

            if (coords == null)
            {               
                Console.WriteLine($"WARN: Unable to geocode address: {address}");
                return null;
            }

            return coords;
        }

        public async Task<(string? City, string? PostCode)> GetCityAndPostalCodeFromAddressAsync(string address)
        {
            return await _mapsApi.GetCityAndPostalCodeFromAddressAsync(address);
        }



        // this is for caltulating distance between two coordinates in kilometers
        public double CalculateDistanceKm(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371;
            var dLat = DegreesToRadians(lat2 - lat1);
            var dLon = DegreesToRadians(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private double DegreesToRadians(double deg) => deg * (Math.PI / 180);
    }

}
