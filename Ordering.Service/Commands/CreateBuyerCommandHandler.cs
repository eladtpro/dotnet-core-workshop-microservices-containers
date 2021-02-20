using System.Threading.Tasks;
using Ordering.Domain.AggregateModels.BuyerAggregate;
using Ordering.Domain.Contracts;

namespace Ordering.API.Commands
{
    public class CreateBuyerCommandHandler
    {
        private readonly IBuyerRepository repository;

        public CreateBuyerCommandHandler(IBuyerRepository buyerRepository)
        {
            repository = buyerRepository;
        }

        public async Task<bool> Handle(CreateBuyerCommand createBuyerCommand)
        {
            // Create Order domain aggregate
            Buyer buyer = new Buyer(createBuyerCommand.OrderId,
                createBuyerCommand.UserName,
                createBuyerCommand.FirstName,
                createBuyerCommand.LastName,
                createBuyerCommand.Address,
                createBuyerCommand.City,
                createBuyerCommand.State,
                createBuyerCommand.PostalCode,
                createBuyerCommand.Country,
                createBuyerCommand.Phone,
                createBuyerCommand.Email);

            //Add Order to DataStore
            await repository.Add(buyer);
            return true;
        }
    }
}