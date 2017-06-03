using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using static System.Console;

namespace dot_net_apps
{
  class Program
  {
    private static ILoyaltyProgramClient client;

    static void Main(string[] args)
    {
      client = new LoyaltyProgramClient("localhost:5000");
      WriteLine();
      WriteLine();
      WriteLine("Welcome to the API Gateway Mock.");
      var cont = true;
      while(cont)
      {
        WriteLine();
        WriteLine();
        WriteLine("********************");
        WriteLine("Choose one of:");
        WriteLine("q <userid> - to query the Loyalty Program Microservice for a user with id <userid>.");
        WriteLine("r <userid> - to register a user with id <userid> with the Loyalty Program Microservice.");
        WriteLine("u <userid> <interests> - to update a user with new comman separated interests");
        WriteLine("t - test call to Loyalty Program Microservice");
        WriteLine("exit - to exit");
        WriteLine("********************");
        var cmd  = ReadLine();
        cont = ProcessCommand(cmd);
      }
    }

    private static bool ProcessCommand(string cmd)
    {
      //TODO: After running the test call command, need to type exit twice to get program to exit
      //first call returns a "did not understand command" message

      if("exit".Equals(cmd))
        return false;

      if(cmd.StartsWith("q"))
        ProcessUserQuery(cmd);
      else if(cmd.StartsWith("r"))
        ProcessUserRegistration(cmd);
      else if(cmd.StartsWith("u"))
        ProcessUpdateUser(cmd);
      else if(cmd.StartsWith("t"))
        ProcessTestCall(cmd);
      else
        WriteLine("Did not understand command");

      return true;
    }

    private static void ProcessTestCall(string cmd)
    {
      var response = client.TestCall().Result;
      PrettyPrintResponse(response);
    }

    private static void ProcessUpdateUser(string cmd)
    {
      throw new NotImplementedException();
    }

    private static void ProcessUserRegistration(string cmd)
    {
      throw new NotImplementedException();
    }

    private static void ProcessUserQuery(string cmd)
    {
      int userid;
      if(!Int32.TryParse(cmd.Substring(1), out userid))
      {
        WriteLine("Please specify user id as an int");
        return;
      }

      var response = client.QueryUser(userid).Result;
      PrettyPrintResponse(response);
    }

    private static async void PrettyPrintResponse(HttpResponseMessage response)
    {
      WriteLine($"Status code: {GetStatusCode(response)}");
      WriteLine($"Headers: {GetPrettyHeaders(response)}");
      WriteLine($"Body: {await response?.Content.ReadAsStringAsync() ?? string.Empty}");
    }

    private static string GetStatusCode(HttpResponseMessage response)
    {
      return response?.StatusCode.ToString() ?? "command failed";
    }

    private static string GetPrettyHeaders(HttpResponseMessage response)
    {
      return response?.Headers.Aggregate("", (acc, h) => {
        var stringArrayDestructed = "";
        if(typeof(string[]).Equals(h.Value.GetType()))
        {
          stringArrayDestructed = string.Join(",", h.Value);
        }
        var headerAndValue = $"{acc}\n\t{h.Key}: {stringArrayDestructed}";
        return headerAndValue;
      }) ?? string.Empty;
    }
  }
}
