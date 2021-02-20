using Ordering.Domain.AggregateModels.BuyerAggregate;
using System.Threading.Tasks;

namespace Ordering.Domain.Contracts
{
    public interface IBuyerRepository
    {
        Task Add(Buyer entity);
    }
}