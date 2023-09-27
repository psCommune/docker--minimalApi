using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Models;
using MinimalApi.Validators;
using System.Diagnostics.Metrics;

namespace MinimalApi
{
    public class Program
    {
        public static void Main (string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddScoped<AbstractValidator<City>, CityValidator>();
            builder.Services.AddDbContext<CitiesContext>(opt => opt.UseSqlite("data source = cities.db"));
            var app = builder.Build();

            app.MapGet("/square/{x}", (int x) => x * x);
            
            app.MapGet("/ekaterinburg", () => new City
            {
                Country = "Россия",
                Name = "Екатеринбург",
                Population = 1539371,
                Id = 1
            });

            app.MapGet("/cities", async (CitiesContext context) => await context.Cities.ToListAsync());

            app.MapPost("/city", async (CitiesContext context, AbstractValidator<City> validator, City city) =>
            {
                ValidationResult results = validator.Validate(city);
                if (!results.IsValid)
                {
                    return Results.ValidationProblem(results.ToDictionary());
                }
                context.Add(city);
                await context.SaveChangesAsync();
                return Results.Ok(city);
            });

            app.MapGet("/city/{id}", async (CitiesContext context, int id) =>
            {
                var found = await context.Cities.FindAsync(id);
                if (found is null)
                {
                    return Results.NotFound();
                }   
                return Results.Ok(found);
            });

            app.MapGet("/cities/{country}", async (CitiesContext context, string country) =>
            {
                var found = context.Cities.Where(i=>i.Country == country);
                if (found is null)
                {
                    return Results.NotFound();
                }
                return Results.Ok(found);
            });

            app.MapPut("/cityEdit/{id}", async (CitiesContext context, AbstractValidator<City> validator, City city, int id) =>
            {
                ValidationResult results = validator.Validate(city);
                City? foundCity = await context.Cities.FindAsync(id);
                if (foundCity is null)
                {
                    return Results.NotFound();
                }
                if (!results.IsValid)
                {
                    return Results.ValidationProblem(results.ToDictionary());
                } else
                {
                    foundCity.Name = city.Name;
                    foundCity.Population = city.Population;
                    await context.SaveChangesAsync();
                    return Results.Ok(foundCity);
                }
               
            });

            app.MapDelete("/cityDelete/{id}", async (CitiesContext context, int id) =>
            {
                var found = await context.Cities.FindAsync(id);
                if (found is null)
                {
                    return Results.NotFound();
                }
                context.Cities.Remove(found);
                await context.SaveChangesAsync();
                return Results.Ok("Ok");
            });

            app.Run();
        }
    }
}