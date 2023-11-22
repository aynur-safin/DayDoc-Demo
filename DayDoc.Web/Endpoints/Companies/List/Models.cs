using DayDoc.Web.Models;
using FastEndpoints;

namespace DayDoc.Web.Endpoints.Models
{
    public class CompanyListRequest : ICommand<CompanyListResponse>
    {
        public string? Filter { get; set; }
        public CompType? CompType { get; set; }
    }

    public class CompanyListResponse
    {
        public List<Company>? Companies { get; set; }
    }
}
