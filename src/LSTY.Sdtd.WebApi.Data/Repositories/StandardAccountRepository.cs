using IceCoffee.DbCore.ExceptionCatch;
using LSTY.Sdtd.WebApi.Data.Entities;
using LSTY.Sdtd.WebApi.Data.IRepositories;
using LSTY.Sdtd.WebApi.Data.Primitives;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LSTY.Sdtd.WebApi.Data.Repositories
{
    public class StandardAccountRepository : DefaultRepository<T_StandardAccount>, IStandardAccountRepository
    {
        public async Task<bool> IsExist(string accountName)
        {
            try
            {
                var result = await base.ExecuteScalarAsync<int>("SELECT 1 FROM T_StandardAccount WHERE AccountName=@AccountName", new
                {
                    AccountName = accountName
                });

                return result == 1;
            }
            catch (Exception ex)
            {
                throw new DbCoreException("检查标准账户名是否存在异常", ex);
            }
        }
    }
}
