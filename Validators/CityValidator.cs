using FluentValidation;
using MinimalApi.Models;

namespace MinimalApi.Validators
{
    public class CityValidator : AbstractValidator<City>
    {
        public CityValidator ()
        {
            RuleFor(city => city.Name).MaximumLength(100);
            RuleFor(city => city.Name).NotEmpty();
            RuleFor(city => city.Population).ExclusiveBetween(1, 1000000000);
        }
    }
}
