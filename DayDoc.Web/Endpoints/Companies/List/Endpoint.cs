using DayDoc.Web.Controllers;
using DayDoc.Web.Data;
using DayDoc.Web.Endpoints.Models;
using DayDoc.Web.Models;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace DayDoc.Web.Endpoints.Companies.List
{
    public class Command : ICommandHandler<CompanyListRequest, CompanyListResponse>
    {
        private readonly AppDbContext _db;

        public Command(AppDbContext db)
        {
            _db = db;
        }

        public async Task<CompanyListResponse> ExecuteAsync(CompanyListRequest req, CancellationToken ct)
        {
            var compQuery = _db.Companies.AsNoTracking();                            

            if (req.CompType != null)
                compQuery = compQuery.Where(m => m.CompType == req.CompType);

            if (!string.IsNullOrEmpty(req.Filter))
                compQuery = compQuery.Where(m => m.Name.Contains(req.Filter));

            var companies = await compQuery
                .Include(m => m.EdoCompany)
                .OrderByDescending(m => m.CompType)
                    .ThenByDescending(m => m.Id)
                .ToListAsync();

            return new CompanyListResponse { Companies = companies };
        }
    }

    public class Endpoint : Endpoint<CompanyListRequest, CompanyListResponse>
    {
        public override void Configure()
        {
            Get("/company/list");
            Description(x => x.WithName("CompanyList"));
            //AllowAnonymous();
        }

        public override async Task HandleAsync(CompanyListRequest req, CancellationToken ct)
        {
            var res = await req.ExecuteAsync(ct);
            await SendAsync(res);
        }
    }
}
