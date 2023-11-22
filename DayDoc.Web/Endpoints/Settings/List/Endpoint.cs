using DayDoc.Web.Controllers;
using DayDoc.Web.Data;
using DayDoc.Web.Endpoints.Models;
using DayDoc.Web.Models;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace DayDoc.Web.Endpoints.Settings.List
{
    public class Command : ICommandHandler<SettingListRequest, SettingListResponse>
    {
        private readonly AppDbContext _db;

        public Command(AppDbContext db)
        {
            _db = db;
        }

        public async Task<SettingListResponse> ExecuteAsync(SettingListRequest req, CancellationToken ct)
        {
            var setQuery = _db.Settings.AsNoTracking();                            

            if (!string.IsNullOrEmpty(req.Filter))
                setQuery = setQuery.Where(m => m.Name != null && m.Name.Contains(req.Filter));

            var settings = await setQuery
                .Include(m => m.OwnCompany)
                .ToListAsync();

            return new SettingListResponse { Settings = settings };
        }
    }

    public class Endpoint : Endpoint<SettingListRequest, SettingListResponse>
    {
        public override void Configure()
        {
            Get("/setting/list");
            Description(x => x.WithName("SettingList"));
            //AllowAnonymous();
        }

        public override async Task HandleAsync(SettingListRequest req, CancellationToken ct)
        {
            var res = await req.ExecuteAsync(ct);
            await SendAsync(res);
        }
    }
}
