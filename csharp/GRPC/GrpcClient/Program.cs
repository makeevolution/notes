// See https://aka.ms/new-console-template for more information

using Grpc.Core;
using Grpc.Net.Client;

namespace GrpcClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var channel = GrpcChannel.ForAddress("http://localhost:5082");
            var input = new HelloRequest { Name = "Aldo" };
            var client = new Greeter.GreeterClient(channel);
            var reply = await client.SayHelloAsync(input);  // Notice that we get an async variant automatically generated
            // by the package :)
            Console.WriteLine(reply.Message);
            
            var channel2 = GrpcChannel.ForAddress("http://localhost:5082");
            var input2 = new CustomerLookupModel() { UserId = 1 };  // output depends on userId supplied; see server
            var client2 = new Customer.CustomerClient(channel2);
            var reply2 = await client2.GetCustomerInfoAsync(input2);  // Notice that we get an async variant automatically generated
            // by the package :)
            Console.WriteLine(reply2.FirstName + reply2.LastName);
            
            using (var call = client2.GetNewCustomers(new NewCustomerRequest()))
            {
                await foreach (var customer in call.ResponseStream.ReadAllAsync())
                {
                    Console.WriteLine(customer.FirstName);
                }
            }
            Console.ReadLine();
        }
    }
}