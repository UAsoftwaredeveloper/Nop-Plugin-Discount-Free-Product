using Nop.Data;
using Nop.Data.DataProviders;
using Nop.Plugin.Misc.FreeProduct.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.FreeProducts.Services
{
    public class SchemaMigration : ISchemaMigration
    {
        #region fields
        private readonly IRepository<FreeProductDiscount> _repository;

        #endregion
        #region Ctor
        public SchemaMigration(IRepository<FreeProductDiscount> repository)
        {
            _repository = repository;
        }

        #endregion
        #region Method()

        public async Task CreateEntity(FreeProductDiscount freeProductDiscount)
        {
            await _repository.InsertAsync(freeProductDiscount, false);
        }
        public async Task Delete(FreeProductDiscount freeProduct)
        {
            await _repository.DeleteAsync(freeProduct, true);
        }
        #endregion
    }
}
