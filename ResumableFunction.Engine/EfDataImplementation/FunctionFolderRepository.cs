using Microsoft.EntityFrameworkCore;
using ResumableFunction.Engine.Abstraction;
using ResumableFunction.Engine.InOuts;

namespace ResumableFunction.Engine.EfDataImplementation
{
    internal class FunctionFolderRepository :RepositoryBase, IFunctionFolderRepository
    {
        public FunctionFolderRepository(EngineDataContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<FunctionFolder>> GetFunctionFolders()
        {
            return await _context.FunctionFolders.ToListAsync();
        }
    }
}
