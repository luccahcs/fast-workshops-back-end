using FastWorkshops.Api.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<Database>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
