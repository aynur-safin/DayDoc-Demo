using DayDoc.Web.Models;
using FastEndpoints;

namespace DayDoc.Web.Endpoints.Models
{
    public class DocDeleteRequest : ICommand
    {
        public int Id { get; set; } 
    }
}
