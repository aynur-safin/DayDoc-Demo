using DayDoc.Web.Controllers;
using DayDoc.Web.Data;
using DayDoc.Web.Endpoints.Models;
using DayDoc.Web.Models;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace DayDoc.Web.Endpoints.Docs.List
{
    public class Command : ICommandHandler<DocListRequest, DocListResponse>
    {
        private readonly AppDbContext _db;

        public Command(AppDbContext db)
        {
            _db = db;
        }

        public async Task<DocListResponse> ExecuteAsync(DocListRequest req, CancellationToken ct)
        {
            var docQuery = _db.Docs.AsNoTracking();                            

            if (req.DocType != null)
                docQuery = docQuery.Where(m => m.DocType == req.DocType);

            if (!string.IsNullOrEmpty(req.Filter))
                docQuery = docQuery.Where(m => m.Name != null && m.Name.Contains(req.Filter));

            var docs = await docQuery
                .Include(m => m.OwnCompany)
                .Include(m => m.Contragent)
                .OrderByDescending(m => m.Id)
                .ToListAsync();

            return new DocListResponse { Docs = docs };
        }
    }

    public class Endpoint : Endpoint<DocListRequest, DocListResponse>
    {
        public override void Configure()
        {
            Get("/doc/list");
            Description(x => x.WithName("DocList"));
            //AllowAnonymous();
        }

        public override async Task HandleAsync(DocListRequest req, CancellationToken ct)
        {
            var res = await req.ExecuteAsync(ct);
            await SendAsync(res);
        }
    }
}
