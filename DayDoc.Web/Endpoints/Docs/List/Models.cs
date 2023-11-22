using DayDoc.Web.Models;
using FastEndpoints;

namespace DayDoc.Web.Endpoints.Models
{
    public class DocListRequest : ICommand<DocListResponse>
    {
        public string? Filter { get; set; }
        public DocType? DocType { get; set; }
    }

    public class DocListResponse
    {
        public List<Doc>? Docs { get; set; }
    }
}
