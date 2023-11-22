using DayDoc.Web.Data;
using DayDoc.Web.Endpoints.Models;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace DayDoc.Web.Endpoints.Docs.Update
{
    public class Command : ICommandHandler<DocUpdateRequest, DocUpdateResponse>
    {
        private readonly AppDbContext _db;

        public Command(AppDbContext db)
        {
            _db = db;
        }

        public async Task<DocUpdateResponse> ExecuteAsync(DocUpdateRequest req, CancellationToken ct)
        {
            _ = req.Doc ?? throw new ArgumentNullException(nameof(req.Doc));

            //_db.Update(req.Doc);
            _db.Entry(req.Doc).State = EntityState.Modified;
            await _db.SaveChangesAsync();         

            return new DocUpdateResponse { Doc = req.Doc };
        }
    }

    public class Endpoint : Endpoint<DocUpdateRequest, DocUpdateResponse>
    {
        public override void Configure()
        {
            Put("/doc/update");
            Description(x => x.WithName("DocUpdate"));
            //AllowAnonymous();
        }

        public override async Task HandleAsync(DocUpdateRequest req, CancellationToken ct)
        {
            var res = await req.ExecuteAsync(ct);
            await SendAsync(res);
        }
    }
}
