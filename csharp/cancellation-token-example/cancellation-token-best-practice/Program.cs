// https://www.youtube.com/watch?v=sWAk4YMK2go

// It is not always a good idea to propagate cancellation token throughout the function body.

using Amazon.S3;
using Amazon.S3.Model;
using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

var app = builder.Build();

app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

#region Large Upload

app.MapPost("/upload-large-file", async (
        [FromForm] FileUploadRequest request, 
        CancellationToken cancellationToken) =>
    {
        var s3Client = new AmazonS3Client(); // reads from ~/.aws/configuration
        try
        {
            await s3Client.PutObjectAsync(new PutObjectRequest()
            {
                BucketName = "myaldosebastianbucket", // Make sure bucket already exists in AWS side
                Key = $"{Guid.NewGuid()} - {request.File.FileName}",
                InputStream = request.File.OpenReadStream()
            }, cancellationToken);

            // await PerformAdditionalTasks(CancellationToken.None);
            await PerformAdditionalTasks(cancellationToken);
            return Results.Ok();
        }
        catch (OperationCanceledException e)
        {
            return Results.StatusCode(499);
        }
        finally
        {
            await s3Client.DeleteObjectAsync( 
                "myaldosebastianbucket", // Make sure bucket already exists in AWS side
                $"{Guid.NewGuid()} - {request.File.FileName}"
            );
        }
    })
    .WithName("UploadLargeFile")
    .DisableAntiforgery()
    .WithOpenApi();

async Task PerformAdditionalTasks(CancellationToken cancellationToken)
{
    var identity = await GetAWSIdentity();
    // This is the learning point. Put debug point on the await below, and cancel the request (e.g. close browser).
    // Also put debug point in catch OperationCanceledException above.
    // If we pass in the request's cancellation token, the publish below will be cancelled; thus our app becomes in an
    // inconsistent state.
    // But if we pass in CancellationToken None, on cancellation, the publish will still be attempted, so less risk
    // of our app being in 
    var snsClient = new AmazonSimpleNotificationServiceClient();
    await snsClient.PublishAsync(new PublishRequest()
    {
        // Go to console of AWS and make sure the topic is available.
        // To test publish, simply create a subscription from your email address there.
        TopicArn = $"arn:aws:sns:eu-north-1:{identity.Arn}:TestTopic",
        Message = "Cancellation token best practices"
    }, cancellationToken);
}

#endregion

app.Run();

# region helpers

async Task<GetCallerIdentityResponse> GetAWSIdentity()
{
    var sts = new AmazonSecurityTokenServiceClient();
    return await sts.GetCallerIdentityAsync(new GetCallerIdentityRequest());
}
record FileUploadRequest(IFormFile File){}
# endregion
