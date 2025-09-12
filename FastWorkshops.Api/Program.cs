var builder = WebApplication.CreateBuilder(args);

// Adiciona os serviços necessários para controllers
builder.Services.AddControllers(); // Adiciona suporte a controllers MVC

var app = builder.Build();

// Configuração do pipeline de requisições

app.UseHttpsRedirection();

// Mapeia os controllers para as rotas da API
app.MapControllers();

app.Run();