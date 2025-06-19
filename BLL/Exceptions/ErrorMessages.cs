using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Exceptions
{
    public static class ErrorMessages
    {
        public const string AccommodationNotFound = "Accommodation {0} not found";

        public const string LandlordNotFound = "Landlord with ID {0} not found";
        public const string CorporateLandlordMissingTaxId = "Corporate landlords must provide tax ID";

        public const string ApplicationNotFound = "Application {0} not found";
        public const string BookingNotFound = "Booking {0} not found.";
        public const string StatusNotFound = "Status '{0}' not found.";
        public const string UnauthorizedBookingModification = "You are not authorized to modify this booking.";

    }

}
