using Grpc.Core;

// Greeter is pulled from the service name in the .proto file
// We have to inherit from Greeter.GreeterBase; GreeterBase part is generated file by framework, stored in obj folder
namespace GrpcServer.Services;

public class CustomersService : Customer.CustomerBase  
{
    private readonly ILogger<GreeterService> _logger;
    public CustomersService(ILogger<GreeterService> logger)
    {
        _logger = logger;
    }

    // We have to override the SayHello method specified in Greeter service inside greet.proto
    public override Task<CustomerModel> GetCustomerInfo(CustomerLookupModel customerRequest, ServerCallContext context)
    {
        var output = new CustomerModel();
        if (customerRequest.UserId.Equals(1))
        {
            output.FirstName = "SomeUser1";
            output.LastName = "LastNameSomeUser1";
        }
        else if (customerRequest.UserId.Equals(2))
        {
            output.FirstName = "SomeUser2";
            output.LastName = "LastNameSomeUser2";
        }
        else
        {
            output.FirstName = "SomeOtherUser";
            output.LastName = "SomeOtherUserLastName";
        }
        return Task.FromResult(output);
    }

    public override async Task GetNewCustomers(NewCustomerRequest request, IServerStreamWriter<CustomerModel> responseStream, ServerCallContext context)
    {
        List<CustomerModel> customers = new List<CustomerModel>
        {
            new CustomerModel
            {
                FirstName = "Tim"
            },
            new CustomerModel
            {
                FirstName = "Tim2"
            },
            new CustomerModel
            {
                FirstName = "Tim3"
            }
        };
        foreach (var cust in customers)
        {
            await responseStream.WriteAsync(cust);
        }
    }
}