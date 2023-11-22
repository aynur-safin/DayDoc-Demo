using DayDoc.Web.Models;
using FastEndpoints;

namespace DayDoc.Web.Endpoints.Models
{
    public class CompanyCreateRequest : ICommand<CompanyCreateResponse>
    {
        public Company? Company { get; set; }
    }

    public class CompanyCreateResponse : CompanyGetResponse
    {
    }

    public class CompanyCreateRequestValidator : AbstractValidator<CompanyCreateRequest>
    {
        public CompanyCreateRequestValidator()
        {
            RuleFor(x => x.Company).SetValidator(new CompanyValidator());
        }
    }
}
