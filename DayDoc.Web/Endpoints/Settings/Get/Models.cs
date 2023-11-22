using DayDoc.Web.Models;
using FastEndpoints;

namespace DayDoc.Web.Endpoints.Models
{
    public class SettingGetRequest : ICommand<SettingGetResponse>
    {
        public int Id { get; set; }     
    }

    public class SettingGetResponse
    {
        public Setting? Setting { get; set; }
    }
}
