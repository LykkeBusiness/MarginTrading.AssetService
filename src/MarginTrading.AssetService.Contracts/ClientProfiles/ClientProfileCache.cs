using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MarginTrading.AssetService.Contracts.ClientProfiles
{
    public class ClientProfileCache : IClientProfileCache
    {
        private readonly IClientProfilesApi _clientProfilesApi;

        private Dictionary<string, ClientProfileContract> _cache =
            new Dictionary<string, ClientProfileContract>();

        private readonly ReaderWriterLockSlim _lockSlim = new ReaderWriterLockSlim();

        public ClientProfileCache(IClientProfilesApi clientProfilesApi)
        {
            _clientProfilesApi = clientProfilesApi;
        }

        public void Start()
        {
            _lockSlim.EnterWriteLock();
            try
            {
                var response = _clientProfilesApi
                    .GetClientProfilesAsync()
                    .GetAwaiter()
                    .GetResult();

                _cache = response.ClientProfiles.ToDictionary(x => x.Id, x => x);
            }
            finally
            {
                _lockSlim.ExitWriteLock();
            }
        }

        public void AddOrUpdate(ClientProfileContract clientProfile)
        {
            _lockSlim.EnterWriteLock();
            try
            {
                _cache[clientProfile.Id] = clientProfile;
            }
            finally
            {
                _lockSlim.ExitWriteLock();
            }
        }

        public void Remove(ClientProfileContract clientProfile)
        {
            _lockSlim.EnterWriteLock();
            try
            {
                var isInCache = _cache.ContainsKey(clientProfile.Id);
                if (isInCache)
                    _cache.Remove(clientProfile.Id);
            }
            finally
            {
                _lockSlim.ExitWriteLock();
            }
        }
    }
}