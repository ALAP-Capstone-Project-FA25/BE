using ALAP.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALAP.BLL.Interface
{
    public interface IUserPackageBizLogic
    {
        Task<UserPackageModel> GetUserPackageByUserId(long userId);
    }
}
