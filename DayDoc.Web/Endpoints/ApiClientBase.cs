using DayDoc.Web.Services;

namespace DayDoc.Web.Endpoints
{
    /*
     * https://stackoverflow.com/questions/40025744/how-to-invoke-a-nswag-client-method-that-needs-bearer-token-on-request-header
     * еще полезная инфа:
     * https://github.com/RicoSuter/NSwag/wiki/CSharpClientGeneratorSettings
     * https://www.meadow.se/wordpress/creating-asp-net-web-apis-and-generate-clients-with-nswag-part-2/
     * https://github.com/Picturepark/Picturepark.SDK.DotNet/blob/master/src/Picturepark.SDK.V1/ClientBase.cs#L11
     */

    public class ApiClientBase
    {
        private readonly TokenProvider _tokenProvider;

        public ApiClientBase(TokenProvider tokenProvider)
        {
            _tokenProvider = tokenProvider;
        }

        protected Task<HttpRequestMessage> CreateHttpRequestMessageAsync(CancellationToken cancellationToken)
        {
            var msg = new HttpRequestMessage();
            // SET THE BEARER AUTH TOKEN
            msg.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _tokenProvider.AccessToken);
            return Task.FromResult(msg);
        }
    }
}
