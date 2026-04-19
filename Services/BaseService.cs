using Microsoft.Extensions.Configuration;

namespace API_AprendeYa.Services
{
    public class BaseService
    {
        protected readonly string _connection;

        protected BaseService(IConfiguration config)
        {
            _connection = config.GetConnectionString("DefaultConnection");
        }
    }
}
