using DayDoc.Web.Controllers;
using DayDoc.Web.Data;
using DayDoc.Web.Endpoints.Models;
using DayDoc.Web.Models;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace DayDoc.Web.Endpoints.Companies.Get
{
    public class Command : ICommandHandler<CompanyGetRequest, CompanyGetResponse>
    {
        private readonly AppDbContext _db;

        public Command(AppDbContext db)
        {
            _db = db;
        }

        public async Task<CompanyGetResponse> ExecuteAsync(CompanyGetRequest req, CancellationToken ct)
        {
            var company = await _db.Companies.AsNoTracking()
                .Include(m => m.EdoCompany)
                .FirstOrDefaultAsync(m => m.Id == req.Id);

            return new CompanyGetResponse { Company = company };
        }
    }

    public class Endpoint : Endpoint<CompanyGetRequest, CompanyGetResponse>
    {
        public override void Configure()
        {
            Get("/company/get/{id}");
            Description(x => x.WithName("CompanyGet"));
            //AllowAnonymous();
        }

        public override async Task HandleAsync(CompanyGetRequest req, CancellationToken ct)
        {
            var res = await req.ExecuteAsync(ct);
            await SendAsync(res);            
        }
    }
}
