using SEAR_DataContract.Misc;
using SEAR_DataContract.Models;
using System.Text.Json;

namespace SEAR_WEB.Misc
{
    public static class ApiCaller
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        internal static class BaseUrl
        {
            public static string Url => "http://localhost:7001/";
        }
        public static async Task<T> CallBackObjectApiAsync<T>(string url, object parameter = null!)
        {
            url = BaseUrl.Url + url;
            HttpClient httpClient = _httpClient;
            HttpResponseMessage? response = null;
            string json = "";
            var watch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                if (parameter == null)
                    response = await httpClient.PostAsync(url, null);
                else
                    response = await httpClient.PostAsJsonAsync(url, parameter);
                
                watch.Stop();
                json = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();
                if (watch.ElapsedMilliseconds > 500)
                {
                    AppLogger.LogInformation($"API Requested URL: {url}, Requested Parameter Object: {parameter ?? "(Empty Object)"} has elapsed milliseconds: {watch.ElapsedMilliseconds}");
                }
            }
            catch (Exception ex)
            {
                if (SEAR_DataContract.Misc.Misc.CheckIsDevelopmentEnvironment())
                {
                    throw CreateAppServerException(url, response, parameter, ex, json);
                }
            }
            return (await response!.Content.ReadFromJsonAsync<T>())!;
        }
        public static async void CallBackApiAsync(string url, object parameter = null!)
        {
            url = BaseUrl.Url + url;
            HttpClient httpClient = _httpClient;
            HttpResponseMessage? response = null;
            string json = "";
            var watch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                if (parameter == null)
                    response = await httpClient.PostAsync(url, null);
                else
                    response = await httpClient.PostAsJsonAsync(url, parameter);

                watch.Stop();
                json = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();
                if (watch.ElapsedMilliseconds > 500)
                {
                    AppLogger.LogInformation($"API Requested URL: {url}, Requested Parameter Object: {parameter ?? "(Empty Object)"} has elapsed milliseconds: {watch.ElapsedMilliseconds}");
                }
            }
            catch (Exception ex)
            {
                if (SEAR_DataContract.Misc.Misc.CheckIsDevelopmentEnvironment())
                {
                    throw CreateAppServerException(url, response, parameter, ex, json);
                }
            }
        }
        private static Exception CreateAppServerException(string url, HttpResponseMessage? response, object parameter, Exception ex, string json)
        {
            parameter = parameter ?? "No Any Parameter";
            return new HttpRequestException(
                $"API call failed.\n" +
                $"URL: {url}\n" +
                $"Response: {(int)response!.StatusCode} {response.ReasonPhrase}\n" +
                $"API Response Content: {JsonSerializer.Deserialize<ApiErrorModel>(json)!.Message}\n" +
                $"Parameter Object: {parameter}\n" +
                $"Exception Message: {ex.Message}"
            );
        }
    }
}