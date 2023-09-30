using HackYeah_API.Database;
using HackYeah_API.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Host.UseSerilog((context, config) => config.ReadFrom.Configuration(context.Configuration));

//builder.Services.AddDbContext<DatabaseContext>(options =>
    //options.UseSqlServer(configuration.GetConnectionString("QueryDatabase")));  // Use UseSqlite for SQLite
builder.Services.AddDbContext<DatabaseContext>(options => 
    options.UseSqlite(configuration.GetConnectionString("SQLitePath")));
builder.Services.AddScoped<SqlQueryExecutor>();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError(exceptionHandlerPathFeature.Error, "Unhandled exception occurred.");

            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("An unexpected error occurred. Please try again later.");
        });
    });
}

if (configuration.GetSection("Kestrel").Exists())
{
    var urls = configuration.GetSection("Kestrel:EndPoints:Http:Url").Value;
    app.Urls.Clear();
    app.Urls.Add(urls);
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<ExceptionMiddleware>();
app.UseSerilogRequestLogging();

app.Run();
