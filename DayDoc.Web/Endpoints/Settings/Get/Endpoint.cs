using DayDoc.Web.Controllers;
using DayDoc.Web.Data;
using DayDoc.Web.Endpoints.Models;
using DayDoc.Web.Models;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace DayDoc.Web.Endpoints.Settings.Get
{
    public class Command : ICommandHandler<SettingGetRequest, SettingGetResponse>
    {
        private readonly AppDbContext _db;

        public Command(AppDbContext db)
        {
            _db = db;
        }

        public async Task<SettingGetResponse> ExecuteAsync(SettingGetRequest req, CancellationToken ct)
        {
            var setting = await _db.Settings.AsNoTracking()
                .Include(m => m.OwnCompany)
                .FirstOrDefaultAsync(m => m.Id == req.Id);

            return new SettingGetResponse { Setting = setting };
        }
    }

    public class Endpoint : Endpoint<SettingGetRequest, SettingGetResponse>
    {
        public override void Configure()
        {
            Get("/doc/setting/get/{id}");
            Description(x => x.WithName("SettingGet"));
            //AllowAnonymous();
        }

        public override async Task HandleAsync(SettingGetRequest req, CancellationToken ct)
        {
            var res = await req.ExecuteAsync(ct);
            await SendAsync(res);
        }
    }
}
