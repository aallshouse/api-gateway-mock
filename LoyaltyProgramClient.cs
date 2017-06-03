using System;
using System.Net.Http;
using System.Threading.Tasks;
using Polly;

public interface ILoyaltyProgramClient
{
  Task<HttpResponseMessage> QueryUser(int userId);
  Task<HttpResponseMessage> TestCall();
}

public class LoyaltyProgramClient : ILoyaltyProgramClient
{
  public LoyaltyProgramClient(string hostName)
  {
    HostName = hostName;
  }
  private static Policy exponentialRetryPolicy =
    Policy
      .Handle<Exception>()
      .WaitAndRetryAsync(
        3,
        attempt => TimeSpan.FromMilliseconds(100 * Math.Pow(2, attempt)),
        (_, __) => Console.WriteLine("retrying..." + _)
      );

  public string HostName { get; private set; }

  public async Task<HttpResponseMessage> QueryUser(int userId)
  {
    return await exponentialRetryPolicy.ExecuteAsync(() => DoUserQuery(userId));
  }

  private async Task<HttpResponseMessage> DoUserQuery(int userId)
  {
    throw new NotImplementedException();
  }

  public async Task<HttpResponseMessage> TestCall()
  {
    var currentDateTimeResource = "/";
    using(var httpClient = new HttpClient())
    {
      httpClient.BaseAddress = new Uri($"http://{HostName}");
      var response = await httpClient.GetAsync(currentDateTimeResource);
      ThrowOnTransientFailure(response);
      return response;
    }
  }

  private static void ThrowOnTransientFailure(HttpResponseMessage response)
  {
    if (((int)response.StatusCode) < 200 || ((int)response.StatusCode) > 499)
      throw new Exception(response.StatusCode.ToString());
  }
}