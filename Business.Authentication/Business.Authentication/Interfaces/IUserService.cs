using Business.Authentication.Models;
using Common.Pagination.Models;
using Data.Authentication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Authentication.Interfaces
{
    public interface IUserService
    {
        Task<PagedList<User>> ListAsync(PaginationParams pagination);
        Task<User> LoadAsync(int id);
        Task<int> SaveAsync(User user);
        Task<int> UpdateAsync(int id, User user);
        Task<int> DeleteAsync(int id);
        Task<AuthenticateResponse> AuthenticateAsync(AuthenticateRequest authRequest);
    }
}
