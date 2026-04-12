using Book_Store_SP.DataAccess.Data;
using Book_Store_SP.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Book_Store_SP.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public ISP_CALL SP_CALL { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            SP_CALL = new SP_Call(_context);  // ← this line is missing
        }
    }
}
