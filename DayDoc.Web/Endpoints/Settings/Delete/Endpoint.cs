using DayDoc.Web.Controllers;
using DayDoc.Web.Data;
using DayDoc.Web.Endpoints.Models;
using DayDoc.Web.Models;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace DayDoc.Web.Endpoints.Settings.Delete
{
    public class Command : ICommandHandler<SettingDeleteRequest>
    {
        private readonly AppDbContext _db;

        public Command(AppDbContext db)
        {
            _db = db;
        }

        public async Task ExecuteAsync(SettingDeleteRequest req, CancellationToken ct)
        {
            var setting = await _db.Settings.FindAsync(req.Id);
            if (setting != null)
            {
                //_db.Settings.Remove(setting);
                _db.Entry(setting).State = EntityState.Deleted;
                await _db.SaveChangesAsync();
            }

            return;
        }
    }

    public class Endpoint : Endpoint<SettingDeleteRequest>
    {
        public override void Configure()
        {
            Delete("/setting/delete/{id}");
            Description(x => x.WithName("SettingDelete"));
            //AllowAnonymous();
        }

        public override async Task HandleAsync(SettingDeleteRequest req, CancellationToken ct)
        {
            await req.ExecuteAsync(ct);
            await SendOkAsync();
        }
    }
}
