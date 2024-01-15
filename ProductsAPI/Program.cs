using Microsoft.OpenApi.Models;
using ProductsAPI.Data;
using ProductsAPI.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// * new instance on new request
builder.Services.AddScoped<DatabaseContext>();

// * when you see that the interface is requested, create a 
// * new instance of PersonRepository
builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Products API", Version = "v1" });
        });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Products API V1");
            });
}

app.MapControllers();

app.Run();