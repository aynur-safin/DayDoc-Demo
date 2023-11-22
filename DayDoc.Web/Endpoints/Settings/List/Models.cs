using DayDoc.Web.Models;
using FastEndpoints;

namespace DayDoc.Web.Endpoints.Models
{
    public class SettingListRequest : ICommand<SettingListResponse>
    {
        public string? Filter { get; set; }
    }

    public class SettingListResponse
    {
        public List<Setting>? Settings { get; set; }
    }
}
