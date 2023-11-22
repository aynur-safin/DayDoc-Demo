using DayDoc.Web.Data;
using DayDoc.Web.Endpoints.Models;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace DayDoc.Web.Endpoints.Settings.Update
{
    public class Command : ICommandHandler<SettingUpdateRequest, SettingUpdateResponse>
    {
        private readonly AppDbContext _db;

        public Command(AppDbContext db)
        {
            _db = db;
        }

        public async Task<SettingUpdateResponse> ExecuteAsync(SettingUpdateRequest req, CancellationToken ct)
        {
            _ = req.Setting ?? throw new ArgumentNullException(nameof(req.Setting));

            //_db.Update(req.Setting);
            _db.Entry(req.Setting).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            return new SettingUpdateResponse { Setting = req.Setting };
        }
    }

    public class Endpoint : Endpoint<SettingUpdateRequest, SettingUpdateResponse>
    {
        public override void Configure()
        {
            Put("/setting/update");
            Description(x => x.WithName("SettingUpdate"));
            //AllowAnonymous();
        }

        public override async Task HandleAsync(SettingUpdateRequest req, CancellationToken ct)
        {
            var res = await req.ExecuteAsync(ct);
            await SendAsync(res);
        }
    }
}
