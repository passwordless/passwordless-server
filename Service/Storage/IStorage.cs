using Fido2NetLib;
using Fido2NetLib.Objects;
using Service.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Storage
{
    public interface IStorage
    {
        Task AddCredentialToUser(Fido2User user, StoredCredential cred);
        Task AddTokenKey(TokenKey tokenKey);
        Task DeleteAccount();
        Task DeleteCredential(byte[] id);
        Task<bool> ExistsAsync(byte[] credentialId);
        Task<AccountMetaInformation> GetAccountInformation();
        Task<ApiKeyDesc> GetApiKeyAsync(string apiKey);
        Task<StoredCredential> GetCredential(byte[] credentialId);
        Task<List<PublicKeyCredentialDescriptor>> GetCredentialsByAliasAsync(string alias);
        Task<List<PublicKeyCredentialDescriptor>> GetCredentialsByUserIdAsync(string userId, bool encodeBase64 = true);
        Task<List<TokenKey>> GetTokenKeys();
        Task LockAllApiKeys(bool isLocked);
        Task RemoveTokenKey(int keyId);
        Task SaveAccountInformation(AccountMetaInformation info);
        Task StoreApiKey(string pkpart, string apikey, string[] scopes);
        Task<bool> TenantExists();
        Task UpdateCredential(byte[] credentialId, uint counter, string country, string device);
        Task<List<UserSummary>> GetUsers(string lastUserId);

        // Aliases
        Task<List<AliasPointer>> GetAliasesByUserId(string userid);
        Task StoreAlias(string userid, Dictionary<string, string> aliases);

        Task<string> GetUserIdByAliasAsync(string alias);


    }
}