using DayDoc.Web.Models;
using FastEndpoints;

namespace DayDoc.Web.Endpoints.Models
{
    public class DocUpdateRequest : ICommand<DocUpdateResponse>
    {
        public Doc? Doc { get; set; }
    }

    public class DocUpdateResponse : DocGetResponse
    {
    }

    public class Validator : AbstractValidator<DocUpdateRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Doc).SetValidator(new DocValidator());
        }
    }
}
