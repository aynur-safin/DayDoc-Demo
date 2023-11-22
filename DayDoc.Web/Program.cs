global using FluentValidation;

using FastEndpoints;
using FastEndpoints.Swagger;

using DayDoc.Web.Data;
using DayDoc.Web.Services;
using Microsoft.AspNetCore.Identity;
using DayDoc.Web.Areas.Identity;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using FastEndpoints.ClientGen;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.Cookies;
using DayDoc.Web.Models;

var builder = WebApplication.CreateBuilder(args);

// DbContext with Identity
//builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddDbContext<AppDbContext>(ServiceLifetime.Transient);
//builder.Services.AddDbContextFactory<AppDbContext>(); // https://learn.microsoft.com/ru-ru/aspnet/core/blazor/blazor-server-ef-core
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<AppDbContext>();
builder.Services.AddAuthentication(options =>
{    
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme; // для перенаправления на страницу Логина
})
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Auth:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Auth:Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Auth:Key"] ?? "")),
            ValidateLifetime = true,
            NameClaimType = "name", // ClaimsIdentity.DefaultNameClaimType
            //RoleClaimType = "role",
            ClockSkew = TimeSpan.FromMinutes(builder.Configuration.GetValue<long>("Auth:ClockSkewMinutes", 5))
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (string.IsNullOrEmpty(context.Token)
                && context.Request.Cookies.ContainsKey("X-Access-Token"))
                {
                    context.Token = context.Request.Cookies["X-Access-Token"];
                }
                return Task.CompletedTask;
            }
        };
    });
// GwtToken to Cookies save
// todo: use refresh token
// https://alimozdemir.medium.com/asp-net-core-jwt-and-refresh-token-with-httponly-cookies-b1b96c849742
// https://github.com/alimozdemir/medium/tree/master/AuthJWTRefresh
// todo: можно убрать Куки-аутентификацию вообще, оставить только bearer через куки
// https://habr.com/ru/articles/468401/
// https://learn.microsoft.com/ru-ru/aspnet/core/blazor/security/server/additional-scenarios?view=aspnetcore-7.0
builder.Services.Configure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme, options =>
{
    options.Events = new CookieAuthenticationEvents
    {
        OnSigningIn = async(context) =>
        {
            var identity = context.Principal?.Identity;
            if (identity != null && identity.IsAuthenticated)
            {                
                var tokenService = context.HttpContext.RequestServices.GetRequiredService<TokenService>();
                var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<AppUser>>();
                //var config = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();

                _ = identity.Name ?? throw new ArgumentNullException(nameof(identity.Name));
                var user = await userManager.FindByEmailAsync(identity.Name);
                if (user != null)
                {
                    _ = user.UserName ?? throw new ArgumentNullException(nameof(user.UserName));
                    var token = tokenService.GetGwtToken(user);

                    user.RefreshToken = Guid.NewGuid().ToString();
                    await userManager.UpdateAsync(user);

                    var cookieOptions = new CookieOptions
                    {
                        MaxAge = TimeSpan.FromMinutes(builder.Configuration.GetValue<long>("Auth:LifetimeMinutes", 1440)),
                        HttpOnly = true,
                        Secure = true,
                        IsEssential = true,
                        SameSite = SameSiteMode.Strict
                    };
                    context.Response.Cookies.Append("X-Access-Token", token, cookieOptions);
                    context.Response.Cookies.Append("X-Username", user.UserName, cookieOptions);
                    context.Response.Cookies.Append("X-Refresh-Token", user.RefreshToken, cookieOptions);
                }
            }
            return;
        },
        OnSigningOut = context =>
        {           
            context.Response.Cookies.Delete("X-Access-Token");
            context.Response.Cookies.Delete("X-Username");
            context.Response.Cookies.Delete("X-Refresh-Token");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddSingleton<TokenService>();
builder.Services.AddScoped<TokenProvider>(); // for blazor pages
var baseUrl = builder.Configuration.GetValue<string>("BaseUrl") ?? throw new ArgumentNullException("BaseUrl");
builder.Services.AddHttpClient<DayDoc.Web.Endpoints.HttpClients.ApiClient>(/*async*/ (serviceProvider, client) =>
{    
    client.BaseAddress = new Uri(baseUrl);

    /*
     * В любом случае, не рекомендуется использовать IHttpContextAccessor в Blazor Server, т.к. это не работает в продакшене
     * https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-context?view=aspnetcore-3.1#avoid-ihttpcontextaccessorhttpcontext-in-razor-components-1
     * Переделал на ApiClientBase.cs
     * см. настройки генерации Nswag в GenerateCSharpClient
     */
});

// MVC
builder.Services.AddControllersWithViews();
// Razor
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddAuthorization();
builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, MyAuthorizationMiddlewareResultHandler>();
// После AddRazorPages и AddServerSideBlazor (https://stackoverflow.com/a/65208268)
builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<AppUser>>();

// For Blazored.FluentValidation with <FluentValidationValidator DisableAssemblyScanning="@true" />
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

// Add FastEndpoints
builder.Services.AddFastEndpoints(o => o.IncludeAbstractValidators = true);
builder.Services.SwaggerDocument(o =>
{
    o.ShortSchemaNames = true;
    o.DocumentSettings = s =>
    {
        var path = Path.Combine(builder.Environment.ContentRootPath, "Endpoints");
        
        s.GenerateCSharpClient(
            settings: s =>
            {
                s.ClassName = "ApiClient";
                s.CSharpGeneratorSettings.Namespace = "DayDoc.Web.Endpoints.HttpClients";
                s.GenerateDtoTypes = false;
                s.AdditionalNamespaceUsages = new string[] { "DayDoc.Web.Services", "DayDoc.Web.Endpoints.Models", "DayDoc.Web.Models", "FastEndpoints" };
                s.UseBaseUrl = false;
                s.ClientBaseClass = "ApiClientBase";
                s.UseHttpRequestMessageCreationMethod = true;
                s.ConfigurationClass = "TokenProvider";
            },
            destination: Path.Combine(path, "ApiClient.cs"));
    };
});

// My Services
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddTransient<XmlDocService>();

var app = builder.Build();

//app.Environment.EnvironmentName = "Production"; // меняем имя окружения

// FastEndpoint ExceptionHandler
app.UseDefaultExceptionHandler();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// https://github.com/dotnet/AspNetCore.Docs/issues/4076#issuecomment-1153254062
app.UseRequestLocalization("ru-RU");

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Use FastEndpoints
app.UseFastEndpoints(c => 
{
    c.Endpoints.RoutePrefix = "api";
    c.Endpoints.ShortNames = true;
});
app.UseSwaggerGen();

app.MapBlazorHub();

app.MapControllerRoute(
    name: "default",
    pattern: "mvc/{controller=Home}/{action=Index}/{id?}");

app.MapFallbackToPage("/_Host");

// Не используется
app.MapGet("/Identity/Account/TokenRefresh", async (context) =>
{
    string? refreshToken = "";
    if (!(context.Request.Cookies.TryGetValue("X-Username", out var userName)
    && context.Request.Cookies.TryGetValue("X-Refresh-Token", out refreshToken)))
        await Results.BadRequest().ExecuteAsync(context);

    var tokenService = context.RequestServices.GetRequiredService<TokenService>();
    var userManager = context.RequestServices.GetRequiredService<UserManager<AppUser>>();
    var config = context.RequestServices.GetRequiredService<IConfiguration>();

    var user = userManager.Users.FirstOrDefault(i => i.UserName == userName && i.RefreshToken == refreshToken);

    if (user != null)
    {
        _ = user.UserName ?? throw new ArgumentNullException(nameof(user.UserName));
        var token = tokenService.GetGwtToken(user);

        user.RefreshToken = Guid.NewGuid().ToString();
        await userManager.UpdateAsync(user);

        var cookieOptions = new CookieOptions
        {
            MaxAge = TimeSpan.FromMinutes(config.GetValue<long>("Auth:LifetimeMinutes", 1440)),
            HttpOnly = true,
            Secure = true,
            IsEssential = true,
            SameSite = SameSiteMode.Strict
        };
        context.Response.Cookies.Append("X-Access-Token", token, cookieOptions);
        context.Response.Cookies.Append("X-Username", user.UserName, cookieOptions);
        context.Response.Cookies.Append("X-Refresh-Token", user.RefreshToken, cookieOptions);

        await Results.Ok().ExecuteAsync(context);
    }

    await Results.BadRequest().ExecuteAsync(context);
});

app.MapGet("/routes", (IEnumerable<EndpointDataSource> endpointSources) =>
{
    var sb = new StringBuilder();
    var endpoints = endpointSources.SelectMany(es => es.Endpoints);
    foreach (var endpoint in endpoints)
    {
        sb.Append(endpoint.DisplayName);

        // получим конечную точку как RouteEndpoint
        if (endpoint is RouteEndpoint routeEndpoint)
        {
            sb.Append(" \t ");
            sb.Append(routeEndpoint.RoutePattern.RawText);
            
            if (endpoint.Metadata.GetMetadata<EndpointNameMetadata>() != null)
            {
                sb.Append(" \t ");
                sb.Append(endpoint.Metadata.GetMetadata<EndpointNameMetadata>()!.EndpointName);
            }

        }

        sb.AppendLine();

        // получение метаданных
        // данные маршрутизации
        // var routeNameMetadata = endpoint.Metadata.OfType<Microsoft.AspNetCore.Routing.RouteNameMetadata>().FirstOrDefault();
        // var routeName = routeNameMetadata?.RouteName;
        // данные http - поддерживаемые типы запросов
        //var httpMethodsMetadata = endpoint.Metadata.OfType<HttpMethodMetadata>().FirstOrDefault();
        //var httpMethods = httpMethodsMetadata?.HttpMethods; // [GET, POST, ...]
    }
    return sb.ToString();
});

app.Run();