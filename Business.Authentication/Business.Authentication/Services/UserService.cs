namespace Business.Authentication.Services
{
    using API.Authentication.Database;
    using Business.Authentication.Extensions;
    using Business.Authentication.Interfaces;
    using Business.Authentication.Models;
    using Common.Encoding.Hash;
    using Common.ExceptionHandler.Exceptions;
    using Common.Pagination;
    using Common.Pagination.Models;
    using Data.Authentication.Globalization.Errors;
    using Data.Authentication.Models;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// User service class
    /// </summary>
    public class UserService : IUserService
    {
        /// <summary>
        /// Users db context
        /// </summary>
        private AuthenticationDbContext _context;

        /// <summary>
        /// Jwt service
        /// </summary>
        private readonly IJwtService _jwtService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="jwtService"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public UserService(AuthenticationDbContext context, IJwtService jwtService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
        }

        /// <summary>
        /// List users
        /// </summary>
        /// <param name="pagination"></param>
        /// <returns>Paginated list with all users</returns>
        public async Task<PagedList<User>> ListAsync(PaginationParams pagination)
        {
            try
            {
                //Get paginated users from db
                List<User> paginatedUsers = await _context.Users.PageBy(x => x.Id, pagination).ToListAsync();

                //Get total count of users in db
                int totalCount = await _context.Users.CountAsync();

                return new PagedList<User>(paginatedUsers, totalCount, pagination.CurrentPage, pagination.PageSize);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Load user
        /// </summary>
        /// <param name="id"></param>
        /// <returns>User</returns>
        public async Task<User> LoadAsync(int id)
        {
            try
            {
                return await _context.Users.SingleOrDefaultAsync(x => x.Id == id);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Save user
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Id</returns>
        public async Task<int> SaveAsync(User user)
        {
            try
            {
                _context.Users.Add(user);

                await _context.SaveChangesAsync();

                return user.Id;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Update user
        /// </summary>
        /// <param name="id"></param>
        /// <param name="user"></param>
        /// <returns>Id</returns>
        public async Task<int> UpdateAsync(int id, User user)
        {
            try
            {
                var oldUser = await LoadAsync(id);

                if (oldUser == null)
                    throw new NotFoundException(Errors.UserNotFound);

                //Update old user fields
                oldUser.UpdateModifiedFields(user, ref _context);

                _context.Update(oldUser);

                await _context.SaveChangesAsync();

                return oldUser.Id;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Delete user
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Id</returns>
        public async Task<int> DeleteAsync(int id)
        {
            try
            {
                var user = await LoadAsync(id);

                if (user == null)
                    throw new NotFoundException(Errors.UserNotFound);

                _context.Remove(user);

                await _context.SaveChangesAsync();

                return user.Id;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Authenticate user
        /// </summary>
        /// <param name="authRequest"></param>
        /// <returns></returns>
        public async Task<AuthenticateResponse> AuthenticateAsync(AuthenticateRequest authRequest)
        {
            try
            {
                var user = await _context.Users.SingleOrDefaultAsync(x => x.Email.ToLower() == authRequest.Email.ToLower());

                // throw exception if user with given email was not found
                if (user == null)
                    throw new NotFoundException(Errors.UserEmailNotFound);

                // throw exception if passwords don't match
                if (user.Password != authRequest.Password.ToSHA256())
                    throw new BadRequestException(Errors.UserPasswordsDontMatch);

                // authentication successful so generate jwt token and return it
                return _jwtService.GenerateJwtToken(user, authRequest.ValidTime);
            }
            catch
            {
                throw;
            }
        }
    }
}
