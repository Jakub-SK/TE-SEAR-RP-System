using SEAR_DataContract.Misc;

namespace SEAR_WEB.AppServer
{
    public class ApiCaller
    {
        //private readonly ILogger<ApiCaller> Logger;
        //public ApiCaller(ILogger<ApiCaller> logger)
        //{
        //    this.Logger = logger;
        //}

        //public async Task<T> CallApi<T>(string url, object parameter)
        //{
        //    HttpClient httpClient = new HttpClient();
        //    var watch = System.Diagnostics.Stopwatch.StartNew();
        //    string callUrl = "https://localhost:7001/" + url;
        //    var response = await httpClient.PostAsJsonAsync(url, parameter);
        //    try
        //    {
        //        response.EnsureSuccessStatusCode();
        //    }
        //    catch
        //    {
        //        throw CreateAppServerException(url, response, parameter);
        //    }
        //    watch.Stop();
        //    if (watch.ElapsedMilliseconds > 500)
        //    {
        //        //Logger.LogInformation("Requested URL: {url}, Requested Value: {value} has exceeded a set of seconds", url, parameter);
        //    }
        //    if (response.IsSuccessStatusCode)
        //    {
        //        return (await response.Content.ReadFromJsonAsync<T>())!;
        //    }
        //    throw CreateAppServerException(url, response, parameter);
        //}

        //Call API
        public T CallApi<T>(string url, object parameter)
        {
            return CallApiAsync<T>(url, parameter).GetAwaiter().GetResult();
        }

        //Don't Call this method directly, use the method above CallApi()
        public async Task<T> CallApiAsync<T>(string url, object parameter)
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
                    Logger logger = new Logger();
                    logger.LogInformation(String.Format("API Requested URL: {0}, Requested Parameter Object: {1} has elapsed milliseconds: {2}", url, (parameter == null ? "(Empty Object)" : parameter), watch.ElapsedMilliseconds.ToString()));
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

        private Exception CreateAppServerException(string url, HttpResponseMessage response, object parameter)
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