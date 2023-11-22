using DayDoc.Web.Data;
using DayDoc.Web.Endpoints.Models;
using DayDoc.Web.Services;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace DayDoc.Web.Endpoints.Docs.XmlDocDelete
{
    public class Command : ICommandHandler<XmlDocDeleteRequest>
    {
        private readonly XmlDocService _xmlDocService;

        public Command(XmlDocService xmlDocService)
        {
            _xmlDocService = xmlDocService;
        }

        public async Task ExecuteAsync(XmlDocDeleteRequest req, CancellationToken ct)
        {
            await _xmlDocService.DeleteXmlDoc(req.DocId, req.Id);
        }
    }

    public class Endpoint : Endpoint<XmlDocDeleteRequest>
    {
        public override void Configure()
        {
            Delete("/doc/xmlDocDelete/{Id}");
            Description(x => x.WithName("XmlDocDelete"));
            //AllowAnonymous();
        }

        public override async Task HandleAsync(XmlDocDeleteRequest req, CancellationToken ct)
        {
            await req.ExecuteAsync(ct);
            await SendOkAsync();
        }
    }
}
