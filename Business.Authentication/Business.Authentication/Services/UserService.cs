namespace Business.Authentication.Services
{
    using Business.Authentication.Enums;
    using Business.Authentication.Interfaces;
    using Business.Authentication.Models;
    using Common.Data.Interfaces;
    using Common.Encoding.Hash;
    using Common.ExceptionHandler.Exceptions;
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
        /// User repository
        /// </summary>
        private readonly IRepository<User> _userRepository;

        /// <summary>
        /// Jwt service
        /// </summary>
        private readonly IJwtService _jwtService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userRepository"></param>
        /// <param name="jwtService"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public UserService(IRepository<User> userRepository, IJwtService jwtService)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
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
                List<User> paginatedUsers = await _userRepository.List(pagination);

                //Get total count of users in db
                int totalCount = await _userRepository.Table.CountAsync();

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
                return await _userRepository.GetById(id);
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
                return await _userRepository.Insert(user);
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
                return await _userRepository.Update(user);
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
                var user = await _userRepository.GetById(id);

                if (user == null)
                    throw new NotFoundException(Errors.UserNotFound);

                return await _userRepository.Delete(user);
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
                switch (authRequest.GrantTypeEnum)
                {
                    case GrantType.Password:
                        var user = await _userRepository.Table.SingleOrDefaultAsync(x => x.Email.ToLower() == authRequest.Email.ToLower());

                        // throw exception if user with given email was not found
                        if (user == null)
                            throw new NotFoundException(Errors.UserEmailNotFound);

                        // throw exception if passwords don't match
                        if (user.Password != authRequest.Password.ToSHA256())
                            throw new BadRequestException(Errors.UserPasswordsDontMatch);

                        // authentication successful so generate jwt token and return it
                        return await _jwtService.GenerateJwtToken(user);
                    case GrantType.Refresh_Token:
                        return null;

                    default:
                        throw new BadRequestException("Current grant type is not allowed by the application");
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
