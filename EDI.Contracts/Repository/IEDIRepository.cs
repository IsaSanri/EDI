using EDI.Contracts.Generics;
using EDI.Entities.Entities;

namespace EDI.Contracts.Repository
{
    public interface IEDIRepository : IGenericActionDbAdd<ItemContainer>, IGenericActionDbUpdate<ItemContainer>, IGenericActionDbQuery<ItemContainer>
    {
    }
}