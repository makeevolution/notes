using System.Net;
using System.Net.Http.Json;
using System.Text;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using OrderCompletion.Api.Core.Ports;
using OrderCompletion.Api.Shared.Errors;
using Xunit;

namespace OrderCompletion.Api.Unit.Tests.Core.Controllers;

public class OrdersControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client; 
    private readonly Mock<IOrderCompletionUseCase> _orderCompletionUseCase = new();
 
    public OrdersControllerTests(WebApplicationFactory<Program> factory)
    {

        var customFactory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddTransient<IOrderCompletionUseCase>(_ => _orderCompletionUseCase.Object);
            });
        });
        _client = customFactory.CreateClient();
    }
    
    [Fact]
    public async Task GivenCompleteOrdersSuccessful_Complete_ReturnsOk()
    {
        // ARRANGE
        var content = new StringContent("[]", Encoding.UTF8, "application/json");
        SetupSuccessCompleteOrder();
        
        // ACT
        var response = await _client.PatchAsync("/Orders", content);
        
        // ASSERT
        Assert.True(response.IsSuccessStatusCode, $"Expected response to succeed, got {response.StatusCode}");
    }

    [Fact]
    public async Task GivenCompleteOrdersFailsDueToValidationError_Complete_ReturnsBadRequest()
    {
        // ARRANGE
        var content = new StringContent("[]", Encoding.UTF8, "application/json");
        SetupFailCompleteOrder(new ValidationError("SomeError"));
        
        // ACT
        var response = await _client.PatchAsync("/Orders", content);
        
        // ASSERT
        Assert.True(response.StatusCode == HttpStatusCode.BadRequest, $"Expected response to fail, got {response.StatusCode}");
    }
    
    [Fact]
    public async Task GivenCompleteOrdersFailsDueToNotFoundError_Complete_ReturnsNotFound()
    {
        // ARRANGE
        var content = new StringContent("[]", Encoding.UTF8, "application/json");
        SetupFailCompleteOrder(new NotFoundError("SomeError"));
        
        // ACT
        var response = await _client.PatchAsync("/Orders", content);
        
        // ASSERT
        Assert.True(response.StatusCode == HttpStatusCode.NotFound, $"Expected response to fail, got {response.StatusCode}");
    }
    
    [Fact]
    public async Task GivenCompleteOrdersFailsDueToUnknownError_Complete_ReturnsInternalServerError()
    {
        // ARRANGE
        var content = new StringContent("[]", Encoding.UTF8, "application/json");
        SetupFailCompleteOrder(new Error("SomeError"));
        
        // ACT
        var response = await _client.PatchAsync("/Orders", content);
        
        // ASSERT
        Assert.True(response.StatusCode == HttpStatusCode.InternalServerError, $"Expected response to fail, got {response.StatusCode}");
    }
    
    [Fact]
    public async Task GivenCompleteOrdersFailsDueToException_Complete_ReturnsInternalServerError()
    {
        // ARRANGE
        var content = new StringContent("[]", Encoding.UTF8, "application/json");
        _orderCompletionUseCase.Setup(x => x.CompleteOrders(It.IsAny<IReadOnlyCollection<int>>(), 
            It.IsAny<CancellationToken>())).Throws(new Exception());        
        // ACT
        var response = await _client.PatchAsync("/Orders", content);
        
        // ASSERT
        Assert.True(response.StatusCode == HttpStatusCode.InternalServerError, $"Expected response to fail, got {response.StatusCode}");
        var resultContent = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.True(resultContent != null, $"Expected result is of type {nameof(ProblemDetails)}");
        Assert.True(resultContent.Title == "An internal server error occurred");
    }
    
    private void SetupSuccessCompleteOrder()
    {
        _orderCompletionUseCase.Setup(x => x.CompleteOrders(It.IsAny<IReadOnlyCollection<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok());
    }
    
    private void SetupFailCompleteOrder(Error error)
    {
        _orderCompletionUseCase.Setup(x => x.CompleteOrders(It.IsAny<IReadOnlyCollection<int>>(), 
            It.IsAny<CancellationToken>())).ReturnsAsync(error);
    }
}