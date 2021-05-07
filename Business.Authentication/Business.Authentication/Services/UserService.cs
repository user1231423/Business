namespace Business.Authentication.Services
{
    using API.Authentication.Database;
    using Business.Authentication.Extensions;
    using Business.Authentication.Models;
    using Common.Encoding.Hash;
    using Common.ExceptionHandler.Exceptions;
    using Common.Pagination;
    using Common.Pagination.Models;
    using Data.Authentication;
    using Data.Authentication.Globalization.Errors;
    using Data.Authentication.Models;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// User service class
    /// </summary>
    public class UserService
    {
        /// <summary>
        /// Users db context
        /// </summary>
        private AuthenticationDbContext _context;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        public UserService(AuthenticationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// List users
        /// </summary>
        /// <param name="pagination"></param>
        /// <returns></returns>
        public PagedList<User> List(PaginationParams pagination)
        {
            try
            {
                return _context.Users.ToPagedList(pagination);
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
        /// <returns></returns>
        public User Load(int id)
        {
            try
            {
                return _context.Users.SingleOrDefault(x => x.Id == id);
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
        /// <returns></returns>
        public async Task<int> Save(User user)
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
        /// <returns></returns>
        public async Task<int> Update(int id, User user)
        {
            try
            {
                var oldUser = Load(id);

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
        /// <returns></returns>
        public async Task<int> Delete(int id)
        {
            try
            {
                var user = Load(id);

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
        public AuthenticateResponse Authenticate(AuthenticateRequest authRequest)
        {
            try
            {
                var user = _context.Users.SingleOrDefault(x => x.Email.ToLower() == authRequest.Email.ToLower() && x.Password == authRequest.Password.ToSHA256());

                // throw exception if user was not found
                if (user == null)
                    throw new NotFoundException(Errors.UserNotFound);

                // authentication successful so generate jwt token
                var token = JWTService.GenerateJwtToken(user, authRequest.ValidTime);

                return new AuthenticateResponse(user, token);
            }
            catch
            {
                throw;
            }
        }
    }
}
