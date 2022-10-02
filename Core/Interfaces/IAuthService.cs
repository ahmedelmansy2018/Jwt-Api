using Core.Models;
using Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IAuthService
    {
        //Auth
        #region Auth
        Task<AuthModel> GetTokenAsync(TokenRequestModel model);

        #endregion
    }
}
