using DayDoc.Web.Data;
using DayDoc.Web.Endpoints.Models;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace DayDoc.Web.Endpoints.Companies.Update
{
    public class Command : ICommandHandler<CompanyUpdateRequest, CompanyUpdateResponse>
    {
        private readonly AppDbContext _db;

        public Command(AppDbContext db)
        {
            _db = db;
        }

        public async Task<CompanyUpdateResponse> ExecuteAsync(CompanyUpdateRequest req, CancellationToken ct)
        {
            _ = req.Company ?? throw new ArgumentNullException(nameof(req.Company));

            //_db.Update(req.Company);
            _db.Entry(req.Company).State = EntityState.Modified; // Изменяем через State, чтобы исключить дочерние свойства
            await _db.SaveChangesAsync();        

            return new CompanyUpdateResponse { Company = req.Company };
        }
    }

    public class Endpoint : Endpoint<CompanyUpdateRequest, CompanyUpdateResponse>
    {
        public override void Configure()
        {
            Put("/company/update");
            Description(x => x.WithName("CompanyUpdate"));
            //AllowAnonymous();
        }

        public override async Task HandleAsync(CompanyUpdateRequest req, CancellationToken ct)
        {
            var res = await req.ExecuteAsync(ct);
            await SendAsync(res);
        }
    }
}
