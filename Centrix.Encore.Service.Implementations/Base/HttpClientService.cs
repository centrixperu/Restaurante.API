using Centrix.Encore.Common.Resources;
using Centrix.Encore.Common.Exceptions;
using Centrix.Encore.IoC;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Centrix.Encore.Service.Implementations.Base
{
    public class HttpClientService
    {
        private Lazy<IHttpClientFactory> factory;
        public HttpClientService()
        {
            this.factory = new Lazy<IHttpClientFactory>(() => IoCContainer.Current.Resolve<IHttpClientFactory>());
        }

        public async Task<T> InvokeAsync<T>(HttpMethod method, string endPoint, string user, string password, object parameters = null)
        {
            HttpClient client = factory.Value.CreateClient();
            var byteArray = Encoding.ASCII.GetBytes(user + ":" + password);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            using (var request = new HttpRequestMessage(method, endPoint))
            {
                if (parameters != null && method != HttpMethod.Get)
                    request.Content = new System.Net.Http.StringContent(JsonConvert.SerializeObject(parameters), Encoding.UTF8, "application/json");

                using (var response = await client.SendAsync(request))
                {
                    if (!response.IsSuccessStatusCode)
                        throw new TechnicalException(CommonResource.httpresponse_500);

                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(content);
                }

            }
        }

        public async Task<List<T>> InvokeAsyncList<T>(HttpMethod method, string endPoint, string user, string password, object parameters = null)
        {
            HttpClient client = factory.Value.CreateClient();
            var byteArray = Encoding.ASCII.GetBytes(user + ":" + password);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            using (var request = new HttpRequestMessage(method, endPoint))
            {
                if (parameters != null && method != HttpMethod.Get)
                    request.Content = new System.Net.Http.StringContent(JsonConvert.SerializeObject(parameters), Encoding.UTF8, "application/json");

                using (var response = await client.SendAsync(request))
                {
                    if (!response.IsSuccessStatusCode)
                        throw new TechnicalException(CommonResource.httpresponse_500);

                    var content = await response.Content.ReadAsStringAsync();
                    //var content2 = JsonConvert.DeserializeObject<T>(content);

                    return JsonConvert.DeserializeObject<List<T>>(content);
                }

            }
        }

    }
}
