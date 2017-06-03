using System.Net.Http;
using System.Threading.Tasks;

public interface ILoyaltyProgramClient
{
  Task<HttpResponseMessage> QueryUser(int userId);
}