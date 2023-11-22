using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DayDoc.Web.Controllers
{
    [Authorize]
    public abstract class _BaseAuthController : Controller
    {
    }
}
