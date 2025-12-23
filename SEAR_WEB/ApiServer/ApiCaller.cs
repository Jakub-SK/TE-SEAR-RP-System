using SEAR_DataContract.Misc;

namespace SEAR_WEB.ApiServer
{
    public static class ApiCaller
    {
        //private readonly ILogger<ApiCaller> Logger;
        //public ApiCaller(ILogger<ApiCaller> logger)
        //{
        //    this.Logger = logger;
        //}

        //Call API
        public static T CallApi<T>(string url, object parameter)
        {
            return CallApiAsync<T>(url, parameter).GetAwaiter().GetResult();
        }

        //Don't Call this method directly, use the method above CallApi()<T>
        private static async Task<T> CallApiAsync<T>(string url, object parameter)
        {
            HttpClient httpClient = new HttpClient();
            url = "https://localhost:7001/" + url;
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var response = await httpClient.PostAsJsonAsync(url, parameter);
            try
            {
                response.EnsureSuccessStatusCode();
                watch.Stop();
                if (watch.ElapsedMilliseconds > 500)
                {
                    Logger.LogInformation(String.Format("API Requested URL: {0}, Requested Parameter Object: {1} has elapsed milliseconds: {2}", url, (parameter == null ? "(Empty Object)" : parameter), watch.ElapsedMilliseconds.ToString()));
                }
                if (response.IsSuccessStatusCode)
                {
                    return (await response.Content.ReadFromJsonAsync<T>())!;
                }
            }
            catch
            {
                throw CreateAppServerException(url, response, parameter);
            }
            throw CreateAppServerException(url, response, parameter);
        }

        private static Exception CreateAppServerException(string url, HttpResponseMessage response, object parameter)
        {
            parameter = parameter == null ? "Empty Parameter" : parameter;
            return new HttpRequestException(
                $"API call failed.\n" +
                $"URL: {url}\n" +
                $"Response: {(int)response.StatusCode} {response.ReasonPhrase}\n" +
                $"Parameter Object: {parameter}"
            );
        }
    }
}