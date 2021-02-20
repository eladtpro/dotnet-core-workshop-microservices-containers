using System.Net.Http;

namespace MusicStore.Helper
{
    public class RestResponse<T> 
    {
        public RestResponse(HttpResponseMessage httpResponseMessage, T data)
        {
            HttpResponseMessage = httpResponseMessage;
            Data = data;
        }
        public HttpResponseMessage HttpResponseMessage { get; }

        public T Data { get; }


    }
}