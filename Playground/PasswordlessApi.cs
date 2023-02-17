using System.Buffers;
using System.Buffers.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;

public static class PasswordlessExtensions
{
    public static string ToBase64Url(this byte[] bytes)
    {
        return PasswordlessApi.Base64Url.Encode(bytes);
    }
}

public class PasswordlessApi
{
    private readonly IHttpClientFactory factory;
    private readonly IOptions<PasswordlessOptions> options;
    public PasswordlessApi(IHttpClientFactory factory, IOptions<PasswordlessOptions> options)
    {
        this.factory = factory;
        this.options = options;
    }

    public async Task<string> CreateRegisterToken(RegisterOptions registerOptions)
    {
        using (var client = SecretClient)
        {
            var res = await client.PostAsJsonAsync("register/token", registerOptions);
            return (await res.Content.ReadAsStringAsync()) ?? "";
        }
    }

    internal async Task<VerifiedUser> VerifyToken(string verifyToken)
    {
        using (var client = SecretClient)
        {
            var req = await client.PostAsJsonAsync("signin/verify", new { token = verifyToken });

            // todo: replace with better error handling
            req.EnsureSuccessStatusCode();

            if (req.IsSuccessStatusCode)
            {
                var res = await req.Content.ReadFromJsonAsync<VerifiedUser>();
                return res;
            }

            return null;
        }
    }

    public async Task<List<PasswordlessUserSummary>> ListUsers()
    {
        using (var client = SecretClient)
        {
            var req = await client.GetAsync("users/list");

            // todo: replace with better error handling
            req.EnsureSuccessStatusCode();

            if (req.IsSuccessStatusCode)
            {
                var res = await req.Content.ReadFromJsonAsync<List<PasswordlessUserSummary>>();
                return res;
            }

            return null;
        }
    }

    public async Task<List<AliasPointer>> ListAliases(string userId)
    {
        using (var client = SecretClient)
        {
            var req = await client.GetAsync($"alias/list?userid={userId}");
            req.EnsureSuccessStatusCode();

            return await req.Content.ReadFromJsonAsync<List<AliasPointer>>();
        }
    }


    public async Task<List<Credential>> ListCredentials(string userId)
    {
        using (var client = SecretClient)
        {
            var req = await client.GetAsync($"credentials/list?userid={userId}");

            req.EnsureSuccessStatusCode();

            return await req.Content.ReadFromJsonAsync<List<Credential>>();
        }
    }

    public async Task DeleteCredential(string id)
    {
        using (var client = SecretClient)
        {
            var req = await client.PostAsJsonAsync("credentials/delete", new { CredentialId = id });

            req.EnsureSuccessStatusCode();
        }
    }

    public async Task DeleteCredential(byte[] id)
    {
        using (var client = SecretClient)
        {
            var req = await client.PostAsJsonAsync("credentials/delete", new { CredentialId = Base64Url.Encode(id) });
            req.EnsureSuccessStatusCode();
        }
    }

    public HttpClient SecretClient
    {
        get
        {
            var client = factory.CreateClient();
            client.BaseAddress = new Uri(options.Value.ApiUrl);
            client.DefaultRequestHeaders.Add("ApiSecret", options.Value.ApiSecret);
            return client;
        }
    }

    public class RegisterOptions
    {
        public string UserId { get; set; }
        public string DisplayName { get; set; }
        public string Username { get; set; }
        public string AttType { get; set; } = "None";
        public string AuthType { get; set; } = "Platform";
        public bool RequireResidentKey { get; set; } = false;
        public string UserVerification { get; set; } = "Preferred";
        public HashSet<string> Aliases { get; set; }
        public bool AliasHashing { get; set; }

    }

    public class PasswordlessUserSummary
    {
        public string UserId { get; set; }
    }

    public class VerifiedUser
    {
        public string UserId { get; set; }
        public bool Success { get; set; }
        public string Username { get; set; }
        public DateTime Timestamp { get; set; }
        public string RpId { get; set; }
        public string Origin { get; set; }
        public string Device { get; set; }
        public string Country { get; set; }
        public string Nickname { get; set; }
        public DateTime ExpiresAt { get; set; }
    }

    public class AuditLog
    {
        public DateTime Timestamp { get; set; }
        public string Level { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
    }

    public class Credential
    {
        public CredentialDescriptor Descriptor { get; set; }
        public byte[] PublicKey { get; set; }
        public byte[] UserHandle { get; set; }
        public uint SignatureCounter { get; set; }
        public string CredType { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid AaGuid { get; set; }
        public DateTime LastUsedAt { get; set; }
        public string RPID { get; set; }
        public string Origin { get; set; }
        public string Country { get; set; }
        public string Device { get; set; }
        public string Nickname { get; set; }
        public string UserId { get; set; }
    }

    public class AliasPointer
    {
        public string UserId { get; set; }
        public string Alias { get; set; }
        public string Plaintext { get; set; }
    }
    public class CredentialDescriptor
    {
        [JsonConverter(typeof(Base64UrlConverter))] public byte[] Id { get; set; }
    }
    public sealed class Base64UrlConverter : JsonConverter<byte[]>
    {
        public override byte[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (!reader.HasValueSequence)
            {
                return Base64Url.DecodeUtf8(reader.ValueSpan);
            }
            return Base64Url.Decode(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, byte[] value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(Base64Url.Encode(value));
        }
    }
    public static class Base64Url
    {
        /// <summary>
        /// Converts arg data to a Base64Url encoded string.
        /// </summary>
        public static string Encode(ReadOnlySpan<byte> arg)
        {
            int minimumLength = (int)(((long)arg.Length + 2L) / 3 * 4);
            char[] array = ArrayPool<char>.Shared.Rent(minimumLength);
            Convert.TryToBase64Chars(arg, array, out var charsWritten);
            Span<char> span = array.AsSpan(0, charsWritten);
            for (int i = 0; i < span.Length; i++)
            {
                ref char reference = ref span[i];
                switch (reference)
                {
                    case '+':
                        reference = '-';
                        break;
                    case '/':
                        reference = '_';
                        break;
                }
            }
            int num = span.IndexOf('=');
            if (num > -1)
            {
                span = span.Slice(0, num);
            }
            string result = new string(span);
            ArrayPool<char>.Shared.Return(array, clearArray: true);
            return result;
        }

        /// <summary>
        /// Decodes a Base64Url encoded string to its raw bytes.
        /// </summary>
        public static byte[] Decode(ReadOnlySpan<char> text)
        {
            int num = (text.Length % 4) switch
            {
                2 => 2,
                3 => 1,
                _ => 0,
            };
            int num2 = text.Length + num;
            char[] array = ArrayPool<char>.Shared.Rent(num2);
            text.CopyTo(array);
            for (int i = 0; i < text.Length; i++)
            {
                ref char reference = ref array[i];
                switch (reference)
                {
                    case '-':
                        reference = '+';
                        break;
                    case '_':
                        reference = '/';
                        break;
                }
            }
            switch (num)
            {
                case 1:
                    array[num2 - 1] = '=';
                    break;
                case 2:
                    array[num2 - 1] = '=';
                    array[num2 - 2] = '=';
                    break;
            }
            byte[] result = Convert.FromBase64CharArray(array, 0, num2);
            ArrayPool<char>.Shared.Return(array, clearArray: true);
            return result;
        }

        /// <summary>
        /// Decodes a Base64Url encoded string to its raw bytes.
        /// </summary>
        public static byte[] DecodeUtf8(ReadOnlySpan<byte> text)
        {
            int num = (text.Length % 4) switch
            {
                2 => 2,
                3 => 1,
                _ => 0,
            };
            int num2 = text.Length + num;
            byte[] array = ArrayPool<byte>.Shared.Rent(num2);
            text.CopyTo(array);
            for (int i = 0; i < text.Length; i++)
            {
                ref byte reference = ref array[i];
                switch (reference)
                {
                    case 45:
                        reference = 43;
                        break;
                    case 95:
                        reference = 47;
                        break;
                }
            }
            switch (num)
            {
                case 1:
                    array[num2 - 1] = 61;
                    break;
                case 2:
                    array[num2 - 1] = 61;
                    array[num2 - 2] = 61;
                    break;
            }
            Base64.DecodeFromUtf8InPlace(array.AsSpan(0, num2), out var bytesWritten);
            byte[] result = array.AsSpan(0, bytesWritten).ToArray();
            ArrayPool<byte>.Shared.Return(array, clearArray: true);
            return result;
        }
    }
}



