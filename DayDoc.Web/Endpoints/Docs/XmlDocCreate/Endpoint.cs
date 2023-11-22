using DayDoc.Web.Endpoints.Models;
using DayDoc.Web.Services;
using FastEndpoints;

namespace DayDoc.Web.Endpoints.Docs.XmlDocCreate
{
    public class Command : ICommandHandler<XmlDocCreateRequest, XmlDocCreateResponse>
    {
        private readonly XmlDocService _xmlDocService;

        public Command(XmlDocService xmlDocService)
        {
            _xmlDocService = xmlDocService;
        }

        public async Task<XmlDocCreateResponse> ExecuteAsync(XmlDocCreateRequest req, CancellationToken ct)
        {
            var xmlDoc = await _xmlDocService.CreateXmlDoc(req.DocId);
            return new XmlDocCreateResponse { XmlDoc = xmlDoc };
        }
    }

    public class Endpoint : Endpoint<XmlDocCreateRequest, XmlDocCreateResponse>
    {
        public override void Configure()
        {
            Post("/doc/xmlDocCreate/{docId}");
            Description(x => x.WithName("XmlDocCreate"));
            //AllowAnonymous();
        }

        public override async Task HandleAsync(XmlDocCreateRequest req, CancellationToken ct)
        {
            var res = await req.ExecuteAsync(ct);
            await SendAsync(res);
        }
    }
}
