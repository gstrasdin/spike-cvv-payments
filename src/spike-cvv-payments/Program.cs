using System;
using System.Linq;
using System.Threading.Tasks;
using Braintree;
using Microsoft.Extensions.Configuration;
using Environment = Braintree.Environment;

namespace Spike.Payments
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var service = new Service(configuration);

            var people =
                configuration.GetSection("Identities:contents:people")
                    .GetChildren()
                    .Select(section =>
                    {
                        var p = new Person();
                        section.Bind(p);
                        return p;
                    })
                    .ToList();

            var person = people[0];

            Task.Run(async () =>
                {
                    var customer = await service.GetCustomerAsync(person) ?? await service.CreateCustomerAsync(person);
                    var token = await service.AddCreditCardAsync(customer.Id, person.CreditCard);
                    var success = await service.ChargeAsync(token, "123", 25.00m);

                    Console.Write("\n\n");
                })
                .Wait();
        }
    }
}