using Microsoft.EntityFrameworkCore;
using ResumableFunction.Abstraction;
using ResumableFunction.Abstraction.InOuts;
using ResumableFunction.Engine.Abstraction;
using ResumableFunction.Engine.InOuts;

namespace ResumableFunction.Engine.EfDataImplementation
{
    internal class FunctionRepository : RepositoryBase, IFunctionRepository
    {
        public FunctionRepository(EngineDataContext dbContext) : base(dbContext)
        {
        }

        public Task<bool> MoveFunctionToRecycleBin(FunctionRuntimeInfo functionRuntimeInfo)
        {
            //throw new NotImplementedException();
            return Task.FromResult(false);
        }

        public async Task<bool> RegisterFunction(ResumableFunctionInstance function, FunctionFolder folder)
        {
            var isExist = (await _context.TypeInfos
                .Where(x => x.Name == function.Name)
                .ToListAsync())
                .FirstOrDefault(x => x.Type == function.GetType());
            if (isExist == null)
            {
                folder.FunctionInfos.Add(new TypeInformation
                {
                    Id = Guid.NewGuid(),
                    Name = function.Name,
                    Type = function.GetType()
                });
            }
            return true;
        }
    }
}
