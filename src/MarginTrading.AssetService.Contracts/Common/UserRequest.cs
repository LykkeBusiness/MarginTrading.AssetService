using System.ComponentModel.DataAnnotations;

namespace MarginTrading.AssetService.Contracts.Common
{
    public class UserRequest
    {
        [Required] public string UserName { get; set; }
    }
}