using Grpc.Core;

// Greeter is pulled from the service name in the .proto file
// We have to inherit from Greeter.GreeterBase; GreeterBase part is generated file by framework, stored in obj folder
namespace GrpcServer.Services;

public class GreeterService : Greeter.GreeterBase  
{
    private readonly ILogger<GreeterService> _logger;
    public GreeterService(ILogger<GreeterService> logger)
    {
        _logger = logger;
    }

    // We have to override the SayHello method specified in Greeter service inside greet.proto
    public override Task<HelloReply> SayHello(HelloRequest helloRequest, ServerCallContext context)
    {
        return Task.FromResult(new HelloReply
        {
            Message = "Hello " + helloRequest.Name  // Notice in the proto, the properties are in lower case, but in
            // csharp, it has to be upper case
        });
    }
}