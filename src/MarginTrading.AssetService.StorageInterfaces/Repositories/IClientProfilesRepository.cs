using System.Collections.Generic;
using System.Threading.Tasks;
using MarginTrading.AssetService.Core.Domain;

namespace MarginTrading.AssetService.StorageInterfaces.Repositories
{
    public interface IClientProfilesRepository
    {
        /// <summary>
        /// Adds new client profile
        /// </summary>
        /// <param name="model"></param>
        /// <param name="clientProfileSettingsToAdd"></param>
        /// <returns>Returns the formerly default client profile which has been updated if the inserted one is the new default</returns>
        Task <ClientProfile> InsertAsync(ClientProfileWithTemplate model, IEnumerable<ClientProfileSettings> clientProfileSettingsToAdd);
        
        /// <summary>
        /// Updates client profile
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Returns the formerly default client profile which has been updated if the updated one is the new default</returns>
        Task<ClientProfile> UpdateAsync(ClientProfile model);
        
        /// <summary>
        /// Deletes client profile
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAsync(string id);
        
        /// <summary>
        /// Gets all client profiles
        /// </summary>
        /// <returns></returns>
        Task<IReadOnlyList<ClientProfile>> GetAllAsync();
        
        /// <summary>
        /// Gets client profile by identifier
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ClientProfile> GetByIdAsync(string id);
        
        /// <summary>
        /// Checks if client profile with provided identifier exists
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> ExistsAsync(string id);
        
        /// <summary>
        /// Checks if provided regulatory profile id is used in any client profile
        /// </summary>
        /// <param name="regulatoryProfileId"></param>
        /// <returns></returns>
        Task<bool> IsRegulatoryProfileAssignedToAnyClientProfileAsync(string regulatoryProfileId);
        
        /// <summary>
        /// Gets default client profile
        /// </summary>
        /// <returns></returns>
        Task<ClientProfile> GetDefaultAsync();
        
        /// <summary>
        /// Gets all client profiles filtered by isDefault property
        /// </summary>
        /// <param name="isDefault"></param>
        /// <returns></returns>
        Task<IReadOnlyList<ClientProfile>> GetByDefaultFilterAsync(bool isDefault);
    }
}