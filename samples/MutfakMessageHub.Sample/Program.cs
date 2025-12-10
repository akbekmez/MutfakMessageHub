using MutfakMessageHub.DependencyInjection;
using MutfakMessageHub.Pipeline;
using MutfakMessageHub.Sample.Common.Behaviors;
using Microsoft.Extensions.DependencyInjection.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure MutfakMessageHub with all features enabled
builder.Services.AddMutfakMessageHub(options =>
{
    options.EnableCaching();
    options.EnableRetry();
    options.EnableOutbox();
    options.EnableTelemetry();
    options.EnableDeadLetterQueue();
    options.PublishParallelByDefault = false; // Sequential by default
});

// Register custom behaviors (open generic registration)
builder.Services.TryAddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
