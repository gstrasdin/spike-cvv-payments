using System;

namespace Spike.Payments
{
    public class Person
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public CreditCard CreditCard { get; set; }
    }

    public class CreditCard
    {
        public string Type { get; set; }
        public string Number { get; set; }
        public string Expiration { get; set; }
        public string Cvv { get; set; }
    }
}