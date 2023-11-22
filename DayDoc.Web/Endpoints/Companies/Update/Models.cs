using DayDoc.Web.Models;
using FastEndpoints;

namespace DayDoc.Web.Endpoints.Models
{
    public class CompanyUpdateRequest : ICommand<CompanyUpdateResponse>
    {
        public Company? Company { get; set; }
    }

    public class CompanyUpdateResponse : CompanyGetResponse
    {
    }

    public class CompanyUpdateRequestValidator : AbstractValidator<CompanyUpdateRequest>
    {
        public CompanyUpdateRequestValidator()
        {
            RuleFor(x => x.Company).SetValidator(new CompanyValidator());
        }
    }
}
