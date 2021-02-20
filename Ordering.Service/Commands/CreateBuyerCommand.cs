using System.Runtime.Serialization;

namespace Ordering.API.Commands
{
    [DataContract]
    public class CreateBuyerCommand
    {
        public CreateBuyerCommand(string orderId,
            string userName,
            string firstName,
            string lastName,
            string address,
            string city,
            string state,
            string postalCode,
            string country,
            string phone,
            string email)
        {
            OrderId = orderId;
            UserName = userName;
            FirstName = firstName;
            LastName = lastName;
            Address = address;
            City = city;
            State = state;
            PostalCode = postalCode;
            Country = country;
            Phone = phone;
            Email = email;
        }

        public string OrderId { get; }
        public string UserName { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string Address { get; }
        public string City { get; }
        public string State { get; }
        public string PostalCode { get; }
        public string Country { get; }
        public string Phone { get; }
        public string Email { get; }
    }
}