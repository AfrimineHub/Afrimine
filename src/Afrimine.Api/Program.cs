using Afrimine.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.RegisterServices(builder.Configuration);

var app = builder.Build();

app.UseApiMiddlewares(builder.Configuration);
app.Run();