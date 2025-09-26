using Eldoria.Application.Common;
using Eldoria.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eldoria.Application.Services
{
    public interface IAuthService
    {
        Task<Result<AuthenticationTokenDto>> AuthenticateUser(string email, string password, CancellationToken ct);
        Task<Result<AuthenticationTokenDto>> RegisterUser(string email, string password, CancellationToken ct);

    }
}
