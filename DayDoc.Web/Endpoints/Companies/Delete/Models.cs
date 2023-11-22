using DayDoc.Web.Models;
using FastEndpoints;

namespace DayDoc.Web.Endpoints.Models
{
    public class CompanyDeleteRequest : ICommand
    {
        public int Id { get; set; } 
    }
}
