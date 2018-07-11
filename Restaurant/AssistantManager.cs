namespace Restaurant {
	public class AssistantManager : IHandle<PriceOrder> {
		private readonly IPublisher _publisher;

		public AssistantManager(IPublisher publisher) {
			_publisher = publisher;
		}

		public void Handle(PriceOrder e) {
			e.Order.Subtotal = 0;

			foreach (var item in e.Order.Items) {
				item.Price = item.Name.Length;
				item.Total = item.Price * item.Quantity;
				e.Order.Subtotal += item.Total;
			}

			e.Order.Tax = e.Order.Subtotal / 5;
			e.Order.Total = e.Order.Subtotal + e.Order.Tax;

			_publisher.Publish(new OrderPriced(e) {Order = e.Order});
		}
	}
}
