using DayDoc.Web.Models;
using FastEndpoints;
using System.Configuration;

namespace DayDoc.Web.Endpoints.Models
{
    public class SettingCreateRequest : ICommand<SettingCreateResponse>
    {
        public Setting? Setting { get; set; }
    }

    public class SettingCreateResponse : SettingGetResponse
    {
    }

    public class SettingCreateRequestValidator : AbstractValidator<SettingCreateRequest>
    {
        public SettingCreateRequestValidator()
        {
            RuleFor(x => x.Setting).SetValidator(new SettingValidator());
        }
    }
}
