using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DataModel
{
    public class AuthRepository : IDisposable
    {
        private readonly UserManager<IdentityUser> _userManager;
        internal AuthContext Context;

        public AuthRepository()
        {
            Context = new AuthContext();
            _userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(Context));
        }

        public void Dispose()
        {
            Context?.Dispose();
            _userManager?.Dispose();
        }

        public IdentityResult RegisterUserSyn(string username, string password, string tel)
        {
            var user = new IdentityUser
            {
                UserName = username,
                PhoneNumber = tel
            };

            var result = _userManager.Create(user, password);

            return result;
        }

        public async Task<IdentityResult> RegisterUser(string username, string password, string tel)
        {
            var user = new IdentityUser
            {
                UserName = username,
                PhoneNumber = tel
            };

            var result = await _userManager.CreateAsync(user, password);

            return result;
        }

        public async Task<IdentityUser> FindUser(string userName, string password)
        {
            var user = await _userManager.FindAsync(userName, password);

            return user;
        }

        public Client FindClient(string clientId)
        {
            return Context.Clients.Find(clientId);
        }

        public async Task<bool> AddRefreshToken(RefreshToken token)
        {
            var existingToken = Context.RefreshTokens.SingleOrDefault(r => r.Subject == token.Subject &&
                                                                           r.ClientId == token.ClientId);

            if (existingToken != null)
            {
                var result = await RemoveRefreshToken(existingToken);
            }

            Context.RefreshTokens.Add(token);

            return await Context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveRefreshToken(RefreshToken token)
        {
            Context.RefreshTokens.Remove(token);
            return await Context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveRefreshToken(string refreshTokenId)
        {
            var refreshToken = await Context.RefreshTokens.FindAsync(refreshTokenId);

            if (refreshToken != null)
            {
                Context.RefreshTokens.Remove(refreshToken);
                return await Context.SaveChangesAsync() > 0;
            }

            return false;
        }

        public async Task<RefreshToken> FindRefreshToken(string refreshTokenId)
        {
            var refreshToken = await Context.RefreshTokens.FindAsync(refreshTokenId);

            return refreshToken;
        }

        public List<RefreshToken> GetAllRefreshTokens()
        {
            return Context.RefreshTokens.ToList();
        }
    }
}
