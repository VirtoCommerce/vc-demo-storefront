using System.Collections.Generic;
using System.Threading.Tasks;

namespace VirtoCommerce.Storefront.Model.Customer.Services
{
    public interface IDemoMemberService
    {
        Task<IDictionary<string, object>> GetMemberIndexByIdAsync(string memberId);
    }
}
