using IceCoffee.DbCore.ExceptionCatch;
using LSTY.Sdtd.WebApi.Data.Entities;
using LSTY.Sdtd.WebApi.Data.IRepositories;
using LSTY.Sdtd.WebApi.Data.Primitives;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LSTY.Sdtd.WebApi.Data.Repositories
{
    public class EmailAccountRepository : DefaultRepository<T_EmailAccount>, IEmailAccountRepository
    {
        public async Task<bool> IsExist(string email)
        {
            try
            {
                var result = await base.ExecuteScalarAsync<int>("SELECT 1 FROM T_EmailAccount WHERE Email=@Email", new
                {
                    Email = email
                });

                return result == 1;
            }
            catch (Exception ex)
            {
                throw new DbCoreException("检查邮箱是否存在异常", ex);
            }
        }
    }
}
