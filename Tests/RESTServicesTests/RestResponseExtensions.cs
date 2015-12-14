namespace Tests.RESTServicesTests
{
    using Newtonsoft.Json;
    using RestSharp;

    public static class RestResponseExtensions
    {
        public static T Deserialize<T>(this IRestResponse response)
        {
            return JsonConvert.DeserializeObject<T>(response.Content);
        }
    }
}
