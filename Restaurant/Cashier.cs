namespace Restaurant {
	public class Cashier : IHandle<TakePayment> {
		private readonly IPublisher _publisher;

		public Cashier(IPublisher publisher) {
			_publisher = publisher;
		}

		public void Handle(TakePayment e) {
			e.Order.Paid = true;
			_publisher.Publish(new OrderPaid(e) {Order = e.Order});
		}
	}
}
