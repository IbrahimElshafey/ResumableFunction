using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumableFunction.Engine.EfDataImplementation
{
    internal class RepositoryBase
    {
        protected readonly EngineDataContext _context;

        public RepositoryBase(EngineDataContext dbContext)
        {
            _context = dbContext;
        }
    }
}
