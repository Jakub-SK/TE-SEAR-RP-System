using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;

namespace SEAR_WEB.AppServer
{
    public class ApiCaller
    {
        private readonly HttpClient HttpClient;
        private readonly ILogger<ApiCaller> Logger;
        public ApiCaller(HttpClient httpClient, ILogger<ApiCaller> logger)
        {
            this.HttpClient = httpClient;
            this.Logger = logger;
        }

        //Call API
        public async Task<T> CallApi<T>(string url, object parameter)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var response = await HttpClient.PostAsJsonAsync(url, parameter);
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch
            {
                throw CreateAppServerException(url, response, parameter);
            }
            watch.Stop();
            if (watch.ElapsedMilliseconds > 500)
            {
                Logger.LogInformation("Requested URL: {url}, Requested Value: {value} has exceeded a set of seconds", url, parameter);
            }
            if (response.IsSuccessStatusCode)
            {
                return (await response.Content.ReadFromJsonAsync<T>())!;
            }
            throw CreateAppServerException(url, response, parameter);
        }

        private Exception CreateAppServerException(string url, HttpResponseMessage response, object value)
        {
            return new HttpRequestException(
                $"API call failed.\n" +
                $"URL: {url}\n" +
                $"Status: {(int)response.StatusCode} {response.ReasonPhrase}\n" +
                $"Response: {value}"
            );
        }
    }
}