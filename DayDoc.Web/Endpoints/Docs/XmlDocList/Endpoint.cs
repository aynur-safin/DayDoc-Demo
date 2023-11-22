using DayDoc.Web.Controllers;
using DayDoc.Web.Data;
using DayDoc.Web.Endpoints.Models;
using DayDoc.Web.Models;
using DayDoc.Web.Services;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace DayDoc.Web.Endpoints.Docs.XmlDocList
{
    public class Command : ICommandHandler<XmlDocListRequest, XmlDocListResponse>
    {
        private readonly XmlDocService _xmlDocService;

        public Command(XmlDocService xmlDocService)
        {
            _xmlDocService = xmlDocService;
        }

        public async Task<XmlDocListResponse> ExecuteAsync(XmlDocListRequest req, CancellationToken ct)
        {
            var xmlDocs = await _xmlDocService.GetXmlDocList(req.DocId);
            return new XmlDocListResponse { XmlDocs = xmlDocs };
        }
    }

    public class Endpoint : Endpoint<XmlDocListRequest, XmlDocListResponse>
    {
        public override void Configure()
        {
            Get("/doc/xmlDocList/{docId}");
            Description(x => x.WithName("XmlDocList"));
            //AllowAnonymous();
        }

        public override async Task HandleAsync(XmlDocListRequest req, CancellationToken ct)
        {
            var res = await req.ExecuteAsync(ct);
            await SendAsync(res);
        }
    }
}
