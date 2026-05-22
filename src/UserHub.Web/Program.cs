using UserHub.Application;
using UserHub.Infrastructure;
using UserHub.Infrastructure.Persistence;
using UserHub.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddWeb(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

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
