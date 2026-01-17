using SEAR_DataContract.Misc;

namespace SEAR_WEB.ApiServer
{
    public static class ApiCaller
    {
        internal static class BaseUrl
        {
            public static string Url => "http://localhost:7001/";
        }
        //Call API
        public static T CallApi<T>(string url)
        {
            return CallApiAsync<T>(url).GetAwaiter().GetResult();
        }
        public static T CallApi<T>(string url, object parameter)
        {
            return CallApiAsync<T>(url, parameter).GetAwaiter().GetResult();
        }
        //Don't Call this method directly, use the method above CallApi()<T>
        private static async Task<T> CallApiAsync<T>(string url)
        {
            url = BaseUrl.Url + url;
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage? response = null;
            var watch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                response = await httpClient.PostAsync(url, null);
                watch.Stop();
                response.EnsureSuccessStatusCode();
                if (watch.ElapsedMilliseconds > 500)
                {
                    AppLogger.LogInformation(String.Format("API Requested URL: {0}, Requested Parameter Object: {1} has elapsed milliseconds: {2}", url, "(Empty Object)", watch.ElapsedMilliseconds.ToString()));
                }
            }
            catch (Exception ex)
            {
                if (Misc.CheckIsDevelopmentEnviroment())
                {
                    throw CreateAppServerException(url, response, null!, ex);
                }
            }
            return (await response!.Content.ReadFromJsonAsync<T>())!;
        }
        private static async Task<T> CallApiAsync<T>(string url, object parameter)
        {
            url = BaseUrl.Url + url;
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage? response = null;
            var watch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                response = await httpClient.PostAsJsonAsync(url, parameter);
                watch.Stop();
                response.EnsureSuccessStatusCode();
                if (watch.ElapsedMilliseconds > 500)
                {
                    AppLogger.LogInformation(String.Format("API Requested URL: {0}, Requested Parameter Object: {1} has elapsed milliseconds: {2}", url, parameter, watch.ElapsedMilliseconds.ToString()));
                }
            }
            catch (Exception ex)
            {
                if (Misc.CheckIsDevelopmentEnviroment())
                {
                    throw CreateAppServerException(url, response, parameter, ex);
                }
            }
            return (await response!.Content.ReadFromJsonAsync<T>())!;
        }
        private static Exception CreateAppServerException(string url, HttpResponseMessage? response, object parameter, Exception ex)
        {
            parameter = parameter == null ? "No Any Parameter" : parameter;
            return new HttpRequestException(
                $"API call failed.\n" +
                $"URL: {url}\n" +
                $"Response: {(int)response!.StatusCode} {response.ReasonPhrase}\n" +
                $"Parameter Object: {parameter}" +
                $"Exception Message: {ex.Message}"
            );
        }
    }
}