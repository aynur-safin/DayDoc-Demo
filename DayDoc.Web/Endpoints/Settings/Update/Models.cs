using DayDoc.Web.Models;
using FastEndpoints;

namespace DayDoc.Web.Endpoints.Models
{
    public class SettingUpdateRequest : ICommand<SettingUpdateResponse>
    {
        public Setting? Setting { get; set; }
    }

    public class SettingUpdateResponse : SettingGetResponse
    {
    }

    public class SettingUpdateRequestValidator : AbstractValidator<SettingUpdateRequest>
    {
        public SettingUpdateRequestValidator()
        {
            RuleFor(x => x.Setting).SetValidator(new SettingValidator());
        }
    }
}
