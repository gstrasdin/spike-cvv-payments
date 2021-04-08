using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Braintree;
using Braintree.Exceptions;
using Microsoft.Extensions.Configuration;
using Environment = Braintree.Environment;

namespace Spike.Payments
{
    public class Service
    {
        private readonly BraintreeGateway _gateway;

        public Service(IConfiguration configuration)
        {
            var gateway = new BraintreeGateway
            {
                Environment = Environment.SANDBOX,
                MerchantId = configuration["Braintree:Gateway:MerchantId"],
                PublicKey = configuration["Braintree:Gateway:PublicKey"],
                PrivateKey = configuration["Braintree:Gateway:PrivateKey"]
            };

            _gateway = gateway;
        }

        public Task<Customer> GetCustomerAsync(Person person)
        {
            Console.Write($"Getting customer {person.Id:N}");
            return _gateway.Customer
                .FindAsync($"{person.Id:N}")
                .ContinueWith(c =>
                {
                    if (c.IsFaulted)
                    {
                        c.Exception.Handle(e => e is NotFoundException);
                        Console.WriteLine("... not found.");
                        return default;
                    }

                    var customer = c.Result;
                    Console.WriteLine($"... found {customer.FirstName} {customer.LastName} (id = {customer.Id}).");
                    return customer;
                });
        }

        public Task<Customer> CreateCustomerAsync(Person person)
        {
            Console.Write($"Creating customer {person.Name}");
            return _gateway.Customer
                .CreateAsync(new CustomerRequest
                {
                    Id = $"{person.Id:N}",
                    Email = person.Email,
                    FirstName = person.Name[..person.Name.IndexOf(' ')],
                    LastName = person.Name[(person.Name.IndexOf(' ') + 1)..]
                })
                .ContinueWith(t =>
                {
                    var customer = t.Result.Target;
                    Console.WriteLine($"... customer created (id = {customer.Id}).");
                    return t.Result.Target;
                });
        }

        public Task<string> AddCreditCardAsync(string customerId, CreditCard creditCard)
        {
            Console.Write($"Adding credit card {creditCard.Number}");
            return _gateway.CreditCard.CreateAsync(new CreditCardRequest
                {
                    CustomerId = customerId,
                    Number = creditCard.Number,
                    ExpirationDate = creditCard.Expiration,
                    CVV = creditCard.Cvv
                })
                .ContinueWith(t =>
                {
                    var token = t.Result.Target.Token;
                    Console.WriteLine($"... credit card added (token = {token}).");
                    
                    return token;
                });
        }

        public Task<bool> ChargeAsync(string token, string cvv, decimal amount)
        {
            Console.Write($"Attempting charge of {amount:C} against payment method with token {token} using CVV {cvv ?? "(NONE)"}");
            return _gateway.Transaction.SaleAsync(new TransactionRequest
                {
                    Amount = amount,
                    PaymentMethodToken = token,
                    PaymentMethodNonce = cvv
                })
                .ContinueWith(t =>
                {
                    var success = t.Result.IsSuccess();
                    Console.WriteLine($"... charge {(success ? "succeeded" : $"failed ({t.Result.Message})")}.");

                    return success;
                });
        }
    }

}