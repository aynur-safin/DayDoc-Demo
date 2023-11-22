using DayDoc.Web.Models;
using FastEndpoints;

namespace DayDoc.Web.Endpoints.Models
{
    public class DocGetRequest : ICommand<DocGetResponse>
    {
        public int Id { get; set; }     
    }

    public class DocGetResponse
    {
        public Doc? Doc { get; set; }
    }
}
