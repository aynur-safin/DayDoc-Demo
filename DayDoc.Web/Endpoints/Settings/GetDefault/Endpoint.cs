using DayDoc.Web.Controllers;
using DayDoc.Web.Data;
using DayDoc.Web.Endpoints.Models;
using DayDoc.Web.Models;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace DayDoc.Web.Endpoints.Settings.GetDefault
{
    public class Command : ICommandHandler<SettingGetDefaultRequest, SettingGetDefaultResponse>
    {
        private readonly AppDbContext _db;

        public Command(AppDbContext db)
        {
            _db = db;
        }

        public async Task<SettingGetDefaultResponse> ExecuteAsync(SettingGetDefaultRequest req, CancellationToken ct)
        {
            var setting = await _db.Settings.AsNoTracking()
                //.Include(m => m.OwnCompany) // Не включать, иначе при Update не обновится OwnCompanyId
                .FirstOrDefaultAsync(/*m => m.Id == req.Id*/);

            return new SettingGetDefaultResponse { Setting = setting };
        }
    }

    public class Endpoint : Endpoint<SettingGetDefaultRequest, SettingGetDefaultResponse>
    {
        public override void Configure()
        {
            Get("/doc/setting/get/default");
            Description(x => x.WithName("SettingGetDefault"));
            //AllowAnonymous();
        }

        public override async Task HandleAsync(SettingGetDefaultRequest req, CancellationToken ct)
        {
            var res = await req.ExecuteAsync(ct);
            await SendAsync(res);
        }
    }
}
