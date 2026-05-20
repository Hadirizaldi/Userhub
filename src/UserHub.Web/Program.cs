using UserHub.Application;
using UserHub.Infrastructure;
using UserHub.Infrastructure.Persistence;
using UserHub.Web.Common.Exceptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddControllers();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.MapGet("/db/ping", async (AppDbContext db) =>
    await db.Database.CanConnectAsync()
        ? Results.Ok("connected")
        : Results.Problem("cannot connect to database"));


app.MapControllers();
app.Run();
