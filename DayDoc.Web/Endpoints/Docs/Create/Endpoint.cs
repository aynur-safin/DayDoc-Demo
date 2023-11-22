using DayDoc.Web.Controllers;
using DayDoc.Web.Data;
using DayDoc.Web.Endpoints.Models;
using DayDoc.Web.Models;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace DayDoc.Web.Endpoints.Docs.Create
{
    public class Command : ICommandHandler<DocsCreateRequest, DocsCreateResponse>
    {
        private readonly AppDbContext _db;

        public Command(AppDbContext db)
        {
            _db = db;
        }

        public async Task<DocsCreateResponse> ExecuteAsync(DocsCreateRequest req, CancellationToken ct)
        {
            _ = req.Doc ?? throw new ArgumentNullException(nameof(req.Doc));

            //_db.Add(req.Doc);
            _db.Entry(req.Doc).State = EntityState.Added;
            await _db.SaveChangesAsync();

            return new DocsCreateResponse { Doc = req.Doc };
        }
    }

    public class Endpoint : Endpoint<DocsCreateRequest, DocsCreateResponse>
    {
        public override void Configure()
        {
            Post("/doc/create");
            Description(x => x.WithName("DocCreate"));
            //AllowAnonymous();
        }

        public override async Task HandleAsync(DocsCreateRequest req, CancellationToken ct)
        {
            var res = await req.ExecuteAsync(ct);

            await SendAsync(res);
            //await SendCreatedAtAsync<Get.Endpoint>(
            //    routeValues: new { Id = res.Doc?.Id },
            //    responseBody: res,
            //    generateAbsoluteUrl: true);
        }
    }
}
