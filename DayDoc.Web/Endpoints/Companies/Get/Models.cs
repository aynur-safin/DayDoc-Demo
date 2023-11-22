using DayDoc.Web.Models;
using FastEndpoints;

namespace DayDoc.Web.Endpoints.Models
{
    public class CompanyGetRequest : ICommand<CompanyGetResponse>
    {
        public int Id { get; set; }     
    }

    public class CompanyGetResponse
    {
        public Company? Company { get; set; }
    }
}
