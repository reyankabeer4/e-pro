using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AirelineReservationSystem.ViewModel
{
    public class RegistrationViewModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
    }
}
