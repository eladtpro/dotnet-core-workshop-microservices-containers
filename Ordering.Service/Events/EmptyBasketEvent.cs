namespace Ordering.API.Events
{
    public class EmptyBasketEvent
    {
        public string BasketID { get; set; }
        public string CorrelationToken { get; set; }
    }
}