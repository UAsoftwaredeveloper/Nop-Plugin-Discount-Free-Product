using FluentValidation;
using Nop.Plugin.Misc.FreeProduct.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Misc.FreeProduct.Validators
{
    /// <summary>
    /// Represents an <see cref="RequirementModel"/> validator.
    /// </summary>
    public class RequirementModelValidator : BaseNopValidator<FreeProductRequirementModel>
    {
        public RequirementModelValidator(ILocalizationService localizationService)
        {
            RuleFor(model => model.DiscountId)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Misc.FreeProducts.Fields.DiscountId.Required"));
            RuleFor(model => model.ProductId)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Misc.FreeProducts.Fields.ProductId.Required"));
            RuleFor(model => model.From)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Misc.FreeProducts.Fields.ProductId.Required"));
            RuleFor(model => model.To)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Misc.FreeProducts.Fields.ProductId.Required"));
            RuleFor(model => model.Published)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Misc.FreeProducts.Fields.Published.Required"));

        }
    }
}
