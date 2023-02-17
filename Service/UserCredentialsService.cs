using Microsoft.Extensions.Configuration;
using Service.Models;
using Service.Storage;
using Service.Helpers;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Service
{
    public class UserCredentialsService
    {
        private readonly IStorage _storage;

        public UserCredentialsService(IStorage storage)
        {
            _storage = storage;
        }

        public UserCredentialsService(string tenant, IConfiguration config, IStorage storage)
        {
            _storage = storage;
        }

        public async Task<StoredCredential[]> GetAllCredentials(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ApiException("userId must not be null or empty", 400);
            }

            var credIds = await _storage.GetCredentialsByUserIdAsync(userId);

            var tasks = credIds.Select(c => _storage.GetCredential(c.Id));
            var creds = await Task.WhenAll(tasks);

            return creds;
        }

        public async Task DeleteCredential(byte[] credentialId)
        {
            if (credentialId == null || credentialId.Length == 0)
            {
                throw new ApiException("credentialId must not be null or empty", 400);
            }
            await _storage.DeleteCredential(credentialId);
        }

        public Task<List<UserSummary>> GetAllUsers(string paginationLastId) {
            return _storage.GetUsers(paginationLastId);
        }
    }

    public class UserSummary {
        public string UserId { get; set; }
    }
}
