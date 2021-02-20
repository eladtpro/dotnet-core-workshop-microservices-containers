using System.Text.RegularExpressions;
using Ordering.Domain.Contracts;

namespace Ordering.Domain.AggregateModels.BuyerAggregate
{
    public class Buyer : IAggregateRoot
    {
        public Buyer(string orderId,
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
            Phone = phone.Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-", "");
            Email = ValidateEmail(email) ? email : string.Empty;
        }

        // DDD Patterns comment
        // Using private fields to encapsulate and carefully manage data.
        // The only way to create an Buyer is through the constructor enabling
        // the domain class to enforce business rules and validation
        public string OrderId { get; private set; }
        public string UserName { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Address { get; private set; }
        public string City { get; private set; }
        public string State { get; private set; }
        public string PostalCode { get; private set; }
        public string Country { get; private set; }
        public string Phone { get; private set; }
        public string Email { get; private set; }

        // This Buyer AggregateRoot's method "ChangeShippingAddress()" should be the only way to change the 
        // shipping information for an order
        public bool ChangeShippingAddress(string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                return false;
            }

            Address = address;
            return true;
        }

        // This Buyer AggregateRoot's method "ChangeShippingCity()" should be the only way to change the 
        // shipping information for an order
        public bool ChangeShippingCity(string city)
        {
            if (string.IsNullOrEmpty(city))
            {
                return false;
            }

            Address = city;
            return true;
        }

        // This Buyer AggregateRoot's method "ChangeShippingState()" should be the only way to change the 
        // shipping information for an order
        public bool ChangeShippingState(string state)
        {
            if (string.IsNullOrEmpty(state))
            {
                return false;
            }

            Address = state;
            return true;
        }

        // This Buyer AggregateRoot's method "ChangeShippingZip()" should be the only way to change the 
        // shipping information for an order
        public bool ChangeShippingZip(string zip)
        {
            if (string.IsNullOrEmpty(zip))
            {
                return false;
            }

            Address = zip;
            return true;
        }

        // This Buyer AggregateRoot's method "ChangeShippingEmailAddress()" should be the only way to change the 
        // shipping information for an order
        public bool ChangeShippingEmailAddress(string emailAddress)
        {
            if (string.IsNullOrEmpty(emailAddress))
            {
                return false;
            }

            Address = emailAddress;
            return true;
        }

        private bool ValidateEmail(string emailAddress)
        {
            return (Regex.IsMatch(emailAddress,
                @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$",
                RegexOptions.IgnoreCase));
        }
    }
}