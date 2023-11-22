using DayDoc.Web.Controllers;
using DayDoc.Web.Data;
using DayDoc.Web.Endpoints.Models;
using DayDoc.Web.Models;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace DayDoc.Web.Endpoints.Companies.Delete
{
    public class Command : ICommandHandler<CompanyDeleteRequest>
    {
        private readonly AppDbContext _db;

        public Command(AppDbContext db)
        {
            _db = db;
        }

        public async Task ExecuteAsync(CompanyDeleteRequest req, CancellationToken ct)
        {
            var company = await _db.Companies.FindAsync(req.Id);
            if (company != null)
            {
                //_db.Companies.Remove(company);
                _db.Entry(company).State = EntityState.Deleted;
                await _db.SaveChangesAsync();
            }

            return;
        }
    }

    public class Endpoint : Endpoint<CompanyDeleteRequest>
    {
        public override void Configure()
        {
            Delete("/company/delete/{id}");
            Description(x => x.WithName("CompanyDelete"));
            //AllowAnonymous();
        }

        public override async Task HandleAsync(CompanyDeleteRequest req, CancellationToken ct)
        {
            await req.ExecuteAsync(ct);
            await SendOkAsync();
        }
    }
}
