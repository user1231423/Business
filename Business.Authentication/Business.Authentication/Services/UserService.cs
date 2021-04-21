using Business.Authentication.Extensions;
using Business.Authentication.Models;
using Common.Encoding.Hash;
using Common.Pagination;
using Common.Pagination.Models;
using Data.Authentication;
using Data.Authentication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Authentication.Services
{
    public class UserService
    {
        /// <summary>
        /// Users db context
        /// </summary>
        private UsersDbContext _context;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        public UserService(UsersDbContext context)
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
            }catch(Exception e)
            {
                throw e;
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
            }catch(Exception e)
            {
                throw e;
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
            }catch(Exception e)
            {
                throw e;
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
                    throw new ArgumentException("User " + id + " not found");

                //Update old user fields
                oldUser.UpdateModifiedFields(user, ref _context);

                _context.Update(oldUser);

                await _context.SaveChangesAsync();

                return oldUser.Id;
            }catch(Exception e)
            {
                throw e;
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
                    throw new ArgumentException("User " + id + " not found");

                _context.Remove(user);

                await _context.SaveChangesAsync();

                return user.Id;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Authenticate user
        /// </summary>
        /// <param name="authRequest"></param>
        /// <returns></returns>
        public async Task<AuthenticateResponse> Authenticate(AuthenticateRequest authRequest)
        {
            try
            {
                var user = _context.Users.SingleOrDefault(x => x.Email.ToLower() == authRequest.Email.ToLower() && x.Password.ToSHA256() == authRequest.Password.ToSHA256());

                // return null if user not found
                if (user == null)
                    throw new ArgumentException("User not found");

                // authentication successful so generate jwt token
                var token = JWTService.GenerateJwtToken(user, authRequest.ValidTime);

                return new AuthenticateResponse(user, token);
            }
            catch(Exception e)
            {
                throw e;
            }
        }
    }
}
