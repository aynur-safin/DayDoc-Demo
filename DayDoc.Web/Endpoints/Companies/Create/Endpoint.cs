using DayDoc.Web.Controllers;
using DayDoc.Web.Data;
using DayDoc.Web.Endpoints.Models;
using DayDoc.Web.Models;
using FastEndpoints;
using FluentResults;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Namotion.Reflection;
using System.Security.Claims;

namespace DayDoc.Web.Endpoints.Companies.Create
{
    public static class CommandEndpointExtension
    {
        public async static Task<bool> CommandAuthorizeAsync(this FastEndpoints.BaseEndpoint endpoint,
            HttpContext? httpContext,
            IEnumerable<EndpointDataSource> endpointSources)
        {
            if (endpoint.Definition == null) // is not Endpoint == is Command
            {
                if (httpContext != null)
                {
                    var origEndpoint = httpContext.GetEndpoint();

                    // get test endpoint
                    var testEndpoint = endpointSources.SelectMany(es => es.Endpoints)
                        .OfType<RouteEndpoint>()
                        .Where(e => e.Metadata.GetMetadata<EndpointDefinition>() != null
                            && e.Metadata.GetMetadata<EndpointDefinition>()!.EndpointType.FullName == endpoint.GetType().FullName)
                        .Where(e => origEndpoint == null
                            || origEndpoint.Metadata.GetMetadata<EndpointDefinition>() == null
                            || origEndpoint.Metadata.GetMetadata<EndpointDefinition>()!.EndpointType.FullName != e.Metadata.GetMetadata<EndpointDefinition>()!.EndpointType.FullName)
                        .FirstOrDefault();

                    if (testEndpoint != null)
                    {
                        try
                        {
                            httpContext.SetEndpoint(testEndpoint);
                            var authenticateResult = await httpContext.AuthenticateAsync();
                            if (!authenticateResult.Succeeded)
                            {
                                if (!httpContext.Response.HasStarted) // is not blazor server connection
                                    await httpContext.Response.SendForbiddenAsync();

                                throw new Exception("Forbidden");
                                //return false;
                            }
                        }
                        finally
                        {
                            httpContext.SetEndpoint(origEndpoint);
                        }
                    }
                }
            }

            return true;
        }
    }


    public class Endpoint : Endpoint<CompanyCreateRequest, CompanyCreateResponse>, ICommandHandler<CompanyCreateRequest, CompanyCreateResponse>
    {
        public override void Configure()
        {
            Post("/company/create");
            Description(x => x.WithName("CompanyCreate"));
            //AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
            //AllowAnonymous();
        }

        private readonly AppDbContext _db;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IEnumerable<EndpointDataSource> _endpointSources;

        public Endpoint(AppDbContext db,
            IHttpContextAccessor contextAccessor,
            IEnumerable<EndpointDataSource> endpointSources)
        {
            _db = db;
            _contextAccessor = contextAccessor;
            _endpointSources = endpointSources;
        }

        public override async Task<CompanyCreateResponse> ExecuteAsync(CompanyCreateRequest req, CancellationToken ct)
        {
            await this.CommandAuthorizeAsync(_contextAccessor.HttpContext, _endpointSources);
           
            _ = req.Company ?? throw new ArgumentNullException(nameof(req.Company));

            //_db.Add(req.Company);
            _db.Entry(req.Company).State = EntityState.Added;
            await _db.SaveChangesAsync();

            return new CompanyCreateResponse { Company = req.Company };
        }

        //public override async Task HandleAsync(Request req, CancellationToken ct)
        //{
        //    var res = await req.ExecuteAsync(ct);
        //    await SendAsync(res);
        //    //await SendCreatedAtAsync<GetProduct.Endpoint>(
        //    //    routeValues: new { Id = res.Id },
        //    //    responseBody: res.Id,
        //    //    generateAbsoluteUrl: true);
        //}
    }
}
