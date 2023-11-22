using DayDoc.Web.Models;
using FastEndpoints;

namespace DayDoc.Web.Endpoints.Models
{
    public class SettingDeleteRequest : ICommand
    {
        public int Id { get; set; } 
    }
}
