using Nop.Plugin.Misc.FreeProduct.Models;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.FreeProducts.Services
{
    public interface ISchemaMigration
    {
        Task CreateEntity(FreeProductDiscount freeProductDiscount);
        Task Delete(FreeProductDiscount freeProduct);
    }
}