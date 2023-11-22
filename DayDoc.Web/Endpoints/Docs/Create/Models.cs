using DayDoc.Web.Models;
using FastEndpoints;

namespace DayDoc.Web.Endpoints.Models
{
    public class DocsCreateRequest : ICommand<DocsCreateResponse>
    {
        public Doc? Doc { get; set; }
    }

    public class DocsCreateResponse : DocGetResponse
    {
    }

    public class DocsCreateRequestValidator : AbstractValidator<DocsCreateRequest>
    {
        public DocsCreateRequestValidator()
        {
            RuleFor(x => x.Doc).SetValidator(new DocValidator());
        }
    }
}
