using Microsoft.EntityFrameworkCore;
using ResumableFunction.Abstraction;
using ResumableFunction.Engine.Abstraction;
using ResumableFunction.Engine.InOuts;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ResumableFunction.Engine.EfDataImplementation
{
    internal class EventProviderRepository : RepositoryBase, IEventProviderRepository
    {
        public EventProviderRepository(EngineDataContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEventProviderHandler> GetByName(string name)
        {
            var typeInfo = await _context.TypeInfos
               .Where(x => x.Name == name)
               .SingleOrDefaultAsync();
            if (typeInfo == null)
            {
                typeInfo = _context.TypeInfos.Local
                    .Where(x => x.Name == name)
                    .SingleOrDefault();
            }
            return typeInfo != null ?
                (IEventProviderHandler)Activator.CreateInstance(typeInfo.Type)! :
                null!;
        }

        public async Task<bool> RegsiterEventProvider(Type eventProviderType, InOuts.FunctionFolder folder)
        {
            var instance = (IEventProviderHandler)Activator.CreateInstance(eventProviderType);
            if (instance is null) return false;
            var isExist = (await _context.TypeInfos
                .Where(x => x.Name == instance.EventProviderName)
                .ToListAsync())
                .FirstOrDefault(x => x.Type == eventProviderType);
            if (isExist == null)
            {
                TypeInformation typeInfo = new TypeInformation
                {
                    Id = Guid.NewGuid(),
                    Name = instance.EventProviderName,
                    Type = eventProviderType
                };
                _context.TypeInfos.Add(typeInfo);
                folder.EventProviderTypes.Add(typeInfo);
            }
            return true;
        }
    }
}
