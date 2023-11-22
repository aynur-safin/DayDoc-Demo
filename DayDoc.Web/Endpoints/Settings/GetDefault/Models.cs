using DayDoc.Web.Models;
using FastEndpoints;

namespace DayDoc.Web.Endpoints.Models
{
    public class SettingGetDefaultRequest : ICommand<SettingGetDefaultResponse>
    {
        //public int Id { get; set; }     
    }

    public class SettingGetDefaultResponse
    {
        public Setting? Setting { get; set; }
    }
}
