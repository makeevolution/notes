// See https://aka.ms/new-console-template for more information

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
            var reply = await client.SayHelloAsync(input);
            Console.WriteLine(reply.Message);
            Console.ReadLine();
        }
    }
}