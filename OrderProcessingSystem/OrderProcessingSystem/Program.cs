using Microsoft.EntityFrameworkCore;
using ORS.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddDbContext<ORSDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Data Source=.;Initial Catalog=OrderProcessingSystem;Integrated Security=True"));
});

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
