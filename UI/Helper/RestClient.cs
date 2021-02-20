using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace MusicStore.Helper
{
    public class RestClient : IRestClient
    {

        //// Best pracitce: Make HttpClient static and reuse.
        //// Creating a new instance for each request is an antipattern that can
        //// result in socket exhaustion.
        private static readonly HttpClient _client;

        static RestClient()
        {
            _client = new HttpClient();
        }

        private readonly string _apiGateway;
        //private readonly HttpClient _client = new HttpClient();

        // Create a TimeSpan of 4 minutes so that HTTP Calls do not timeout when debugging
        // Do not do this in production!!!
        private readonly TimeSpan _httpTimeOut = new TimeSpan(0, 4, 0);

        public RestClient(IConfiguration config)
        {
            _apiGateway = config["ApiGateway"];
            _client.Timeout = _httpTimeOut;
        }

        public async Task<RestResponse<TReturnMessage>> GetAsync<TReturnMessage>(string path)
            where TReturnMessage : class, new()
        {
            // robvet, 6-28-18, removed "http" prefix constant as we now get this
            // directly from the configuration file
            //var uri = new Uri($"http://{_apiGateway}/{path}");
            var uri = new Uri($"{_apiGateway}/{path}");

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await _client.GetAsync(uri);

            if (!response.IsSuccessStatusCode)
            {
                var ex = new HttpRequestException($"Error: StatusCode: {response.StatusCode} - Message: {response.ReasonPhrase}");
                ex.Data.Add("StatusCode", response.StatusCode);
                ex.Data.Add("ReasonPhrase", response.ReasonPhrase);
                return new RestResponse<TReturnMessage>(response, null);
                //throw new Exception(ex.Message);
            }

            var result = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<TReturnMessage>(result);
            return new RestResponse<TReturnMessage>(response, data);
        }

        public async Task<RestResponse<TReturnMessage>> PostAsync<TReturnMessage>(string path, object dataObject = null)
            where TReturnMessage : class, new()

        {
            //var uri = new Uri($"http://{_apiGateway}/{path}");
            var uri = new Uri($"{_apiGateway}/{path}");


            string content = dataObject == null ? "{}" : JsonConvert.SerializeObject(dataObject);

            HttpResponseMessage response = await _client.PostAsync(uri, new StringContent(content, Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode)
            {
                var ex = new HttpRequestException($"Error: StatusCode: {response.StatusCode} - Message: {response.ReasonPhrase}");
                ex.Data.Add("StatusCode", response.StatusCode);
                ex.Data.Add("ReasonPhrase", response.ReasonPhrase);
                throw new Exception(ex.Message);
            }

            var result = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<TReturnMessage>(result);
            return new RestResponse<TReturnMessage>(response, data);
        }

        public async Task<RestResponse<TReturnMessage>> PutAsync<TReturnMessage>(string path, object dataObject = null)
            where TReturnMessage : class, new()
        {
            var uri = new Uri($"{_apiGateway}/{path}");

            string content = dataObject != null ? JsonConvert.SerializeObject(dataObject) : "{}";

            HttpResponseMessage response = await _client.PutAsync(uri, new StringContent(content, Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode)
            {
                var ex = new HttpRequestException($"Error: StatusCode: {response.StatusCode} - Message: {response.ReasonPhrase}");
                ex.Data.Add("StatusCode", response.StatusCode);
                ex.Data.Add("ReasonPhrase", response.ReasonPhrase);
                throw new Exception(ex.Message);
            }

            var result = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<TReturnMessage>(result);
            return new RestResponse<TReturnMessage>(response, data);
        }

        public async Task<RestResponse<bool>> DeleteAsync(string path)
        {
            var uri = new Uri($"{_apiGateway}/{path}");

            HttpResponseMessage response = await _client.DeleteAsync(uri);

            //if (!response.IsSuccessStatusCode)
            //{
            //    var ex = new HttpRequestException(
            //        $"Error: StatusCode: {response.StatusCode} - Message: {response.ReasonPhrase}");
            //    ex.Data.Add("StatusCode", response.StatusCode);
            //    ex.Data.Add("ReasonPhrase", response.ReasonPhrase);
            //    throw new Exception(ex.Message);
            //}

            return new RestResponse<bool>(response, response.IsSuccessStatusCode);
        }
    }
}