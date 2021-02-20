using System;
using System.Text;
using System.Threading.Tasks;
using EventBus.EventBus;
using EventBus.Events;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using Ordering.API.Commands;

namespace Ordering.API.Events
{
    public class UserCheckoutEventHandler : IIntegratedEventHandler
    {
        private readonly CreateBuyerCommandHandler _buyerCommandHandler;
        private readonly IEventBus _eventBus;
        private readonly CreateOrderCommandHandler _orderCommandHandler;

        public UserCheckoutEventHandler(IEventBus eventBus,
            CreateOrderCommandHandler orderCommandHandler,
            CreateBuyerCommandHandler buyerCommandHandler)
        {
            _eventBus = eventBus;
            _orderCommandHandler = orderCommandHandler;
            _buyerCommandHandler = buyerCommandHandler;
        }

        public async Task HandleAsync(Message message)
        {
            try
            {
                // Interrogate the message
                // Get the Event Type
                message.UserProperties.TryGetValue("Event", out var eventType);

                // Get the CorrelationToken
                message.UserProperties.TryGetValue("correlationToken", out var correlationToken);

                // Get the message body
                var body = Encoding.UTF8.GetString(message.Body);
                
                // Deserialize the message body into the event class
                var checkOut = JsonConvert.DeserializeObject<UserCheckoutEvent>(body);

                // Create Command (data) object that follows simple CQRS pattern
                var createOrderCommand = new CreateOrderCommand(
                    checkOut.CustomerId,
                    checkOut.BasketId,
                    checkOut.OrderDate,
                    checkOut.UserName,
                    checkOut.Total,
                    checkOut.OrderDetails);

                // Invoke Command that creates order
                var orderId = await _orderCommandHandler.Handle(createOrderCommand);

                // Create Buyer Command (data) object that follows simple CQRS pattern
                var createBuyerCommand = new CreateBuyerCommand(orderId,
                    checkOut.UserName,
                    checkOut.BuyerInformation.FirstName,
                    checkOut.BuyerInformation.LastName,
                    checkOut.BuyerInformation.Address,
                    checkOut.BuyerInformation.City,
                    checkOut.BuyerInformation.State,
                    checkOut.BuyerInformation.PostalCode,
                    checkOut.BuyerInformation.Country,
                    checkOut.BuyerInformation.Phone,
                    checkOut.BuyerInformation.Email);

                // Invoke Command that creates buyer
                await _buyerCommandHandler.Handle(createBuyerCommand);

                //************** Publish Event  *************************
                // Publish event to clear basket for this order from Basket service
                var emptyCartEvent = new EmptyBasketEvent
                {
                    BasketID = checkOut.BasketId,
                    CorrelationToken = correlationToken?.ToString()
                };

                await _eventBus.Publish(emptyCartEvent,
                    MessageEventEnum.InvokeEmptyBasketEvent,
                    correlationToken?.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception executing UserCheckoutEvent in Eventhandler : {ex.Message}");
            }

            await Task.CompletedTask;
        }
    }
}