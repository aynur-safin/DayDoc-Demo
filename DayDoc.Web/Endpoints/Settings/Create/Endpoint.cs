using DayDoc.Web.Controllers;
using DayDoc.Web.Data;
using DayDoc.Web.Endpoints.Models;
using DayDoc.Web.Models;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace DayDoc.Web.Endpoints.Settings.Create
{
    public class Command : ICommandHandler<SettingCreateRequest, SettingCreateResponse>
    {
        private readonly AppDbContext _db;

        public Command(AppDbContext db)
        {
            _db = db;
        }

        public async Task<SettingCreateResponse> ExecuteAsync(SettingCreateRequest req, CancellationToken ct)
        {
            _ = req.Setting ?? throw new ArgumentNullException(nameof(req.Setting));

            //_db.Add(req.Setting);
            _db.Entry(req.Setting).State = EntityState.Added;
            await _db.SaveChangesAsync();

            return new SettingCreateResponse { Setting = req.Setting };
        }
    }

    public class Endpoint : Endpoint<SettingCreateRequest, SettingCreateResponse>
    {
        public override void Configure()
        {
            Post("/setting/create");
            Description(x => x.WithName("SettingCreate"));
            //AllowAnonymous();
        }

        public override async Task HandleAsync(SettingCreateRequest req, CancellationToken ct)
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
