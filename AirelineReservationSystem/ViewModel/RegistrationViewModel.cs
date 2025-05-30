using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AirelineReservationSystem.ViewModel
{
    public class RegistrationViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Address{ get; set; }
        public string Sex{ get; set; }
        public string Age { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
    }
}
