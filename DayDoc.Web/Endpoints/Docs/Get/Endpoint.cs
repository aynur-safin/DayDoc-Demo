using DayDoc.Web.Controllers;
using DayDoc.Web.Data;
using DayDoc.Web.Endpoints.Models;
using DayDoc.Web.Models;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace DayDoc.Web.Endpoints.Docs.Get
{
    public class Command : ICommandHandler<DocGetRequest, DocGetResponse>
    {
        private readonly AppDbContext _db;

        public Command(AppDbContext db)
        {
            _db = db;
        }

        public async Task<DocGetResponse> ExecuteAsync(DocGetRequest req, CancellationToken ct)
        {
            var doc = await _db.Docs.AsNoTracking()
                .Include(m => m.OwnCompany)
                .Include(m => m.Contragent)
                //.Include(m => m.XmlDocs)
                .FirstOrDefaultAsync(m => m.Id == req.Id);

            return new DocGetResponse { Doc = doc };
        }
    }

    public class Endpoint : Endpoint<DocGetRequest, DocGetResponse>
    {
        public override void Configure()
        {
            Get("/doc/get/{id}");
            Description(x => x.WithName("DocGet"));
            //AllowAnonymous();
        }

        public override async Task HandleAsync(DocGetRequest req, CancellationToken ct)
        {
            var res = await req.ExecuteAsync(ct);
            await SendAsync(res);
        }
    }
}
