using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.Extensions.Hosting;

namespace bank_api.Auth
{
    public class UserStore
    {
        private readonly string usersFilePath;
        private readonly SemaphoreSlim fileLock = new(1, 1);
        private readonly ConcurrentDictionary<string, StoredUser> usersByUsername = new(StringComparer.OrdinalIgnoreCase);

        public UserStore(IHostEnvironment env)
        {
            string dataDir = Path.Combine(env.ContentRootPath, "Data");
            Directory.CreateDirectory(dataDir);
            usersFilePath = Path.Combine(dataDir, "users.json");
            LoadAsync().GetAwaiter().GetResult();
        }

        public Task<bool> ExistsAsync(string username) => Task.FromResult(usersByUsername.ContainsKey(username));

        public Task<StoredUser?> GetAsync(string username)
        {
            usersByUsername.TryGetValue(username, out var user);
            return Task.FromResult(user);
        }

        public async Task AddAsync(StoredUser user)
        {
            if (!usersByUsername.TryAdd(user.Username, user))
            {
                throw new InvalidOperationException("User already exists");
            }
            await SaveAsync();
        }

        private async Task LoadAsync()
        {
            if (!File.Exists(usersFilePath))
            {
                return;
            }

            await fileLock.WaitAsync();
            try
            {
                using FileStream fs = File.OpenRead(usersFilePath);
                var list = await JsonSerializer.DeserializeAsync<List<StoredUser>>(fs) ?? new List<StoredUser>();
                foreach (var u in list)
                {
                    usersByUsername[u.Username] = u;
                }
            }
            finally
            {
                fileLock.Release();
            }
        }

        private async Task SaveAsync()
        {
            await fileLock.WaitAsync();
            try
            {
                var list = usersByUsername.Values.OrderBy(u => u.Username, StringComparer.OrdinalIgnoreCase).ToList();
                var options = new JsonSerializerOptions { WriteIndented = true };
                using FileStream fs = File.Create(usersFilePath);
                await JsonSerializer.SerializeAsync(fs, list, options);
            }
            finally
            {
                fileLock.Release();
            }
        }
    }

    public record StoredUser(string Username, string PasswordHash);
}
