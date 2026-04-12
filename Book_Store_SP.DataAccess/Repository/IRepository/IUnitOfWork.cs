using System;
using System.Collections.Generic;
using System.Text;

namespace Book_Store_SP.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork 
    {
        ISP_CALL SP_CALL { get; }
    }
}
