using Restaurant;
using Xunit;

namespace Restaurants.Tests
{
	public class CashierTests {
		[Fact]
		public void produces_paid_event_when_food_is_paid_for() {
			var order = new Order();

			var dispatcher = new FakeDispatcher();
			var cashier = new Cashier(dispatcher);

			cashier.Handle(new TakePayment(null) {
				Order = order
			});

			Assert.Equal(1, dispatcher.Messages.Count);

			var evnt = dispatcher.Messages[0];
			Assert.IsType<OrderPaid>(evnt);
		}
	}
}
