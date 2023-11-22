using DayDoc.Web.Controllers;
using DayDoc.Web.Data;
using DayDoc.Web.Endpoints.Models;
using DayDoc.Web.Models;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace DayDoc.Web.Endpoints.Docs.Delete
{
    public class Command : ICommandHandler<DocDeleteRequest>
    {
        private readonly AppDbContext _db;

        public Command(AppDbContext db)
        {
            _db = db;
        }

        public async Task ExecuteAsync(DocDeleteRequest req, CancellationToken ct)
        {
            var doc = await _db.Docs.FindAsync(req.Id);
            if (doc != null)
            {
                //_db.Docs.Remove(doc);
                _db.Entry(doc).State = EntityState.Deleted;
                await _db.SaveChangesAsync();
            }

            return;
        }
    }

    public class Endpoint : Endpoint<DocDeleteRequest>
    {
        public override void Configure()
        {
            Delete("/doc/delete/{id}");
            Description(x => x.WithName("DocDelete"));
            //AllowAnonymous();
        }

        public override async Task HandleAsync(DocDeleteRequest req, CancellationToken ct)
        {
            await req.ExecuteAsync(ct);
            await SendOkAsync();
        }
    }
}
