using DayDoc.Web.Controllers;
using DayDoc.Web.Data;
using DayDoc.Web.Endpoints.Models;
using DayDoc.Web.Models;
using DayDoc.Web.Services;
using FastEndpoints;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace DayDoc.Web.Endpoints.Docs.XmlDocGetFile
{
    public class Command : ICommandHandler<XmlDocGetFileRequest, XmlDocGetFileInfoResponse?>
    {
        private readonly XmlDocService _xmlDocService;

        public Command(XmlDocService xmlDocService)
        {
            _xmlDocService = xmlDocService;
        }

        public async Task<XmlDocGetFileInfoResponse?> ExecuteAsync(XmlDocGetFileRequest req, CancellationToken ct)
        {
            var fileInfo = await _xmlDocService.GetXmlDocFileInfo(req.Id);

            if(fileInfo == null)
                return null;

            return new XmlDocGetFileInfoResponse { FileName = fileInfo.Name, FilePath = fileInfo.FullName };
        }
    }

    public class Endpoint : Endpoint<XmlDocGetFileRequest>
    {
        public override void Configure()
        {
            Get("/doc/xmlDocGetFile/{id}");
            Description(x => x.WithName("XmlDocGetFile"));
            //AllowAnonymous();
        }

        public override async Task HandleAsync(XmlDocGetFileRequest req, CancellationToken ct)
        {
            var file = await req.ExecuteAsync(ct);
            if (file == null || file.FilePath == null)
            {
                await SendNotFoundAsync();
                return;
            }

            await SendFileAsync(new FileInfo(file.FilePath), file.ContentType);
        }
    }
}
