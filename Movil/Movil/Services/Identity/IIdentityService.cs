using System.Threading.Tasks;

namespace Movil.Services.Identity
{

    interface IIdentityService
    {
        Task<bool> VerifyRegistration();
        Task Authenticate();
    }
}
