using System;
using Restaurant;
using Xunit;

namespace Restaurants.Tests {
	public class RunnerTests {
		[Fact]
		public void after_first_attempt_failed_on_callback_retry() {
			// given
			var d = new FakeDispatcher();
			var m = new Runner(d);
			var o = new Order {OrderId = Guid.NewGuid()};

			m.Handle(new OrderPlaced {Order = o});
			var cookFoodCallback1 = (d.Messages[1] as Callback).Payload as CookFoodCalledBack;
			d.Clear();

			// when
			var inputEvent = cookFoodCallback1;
			m.Handle(inputEvent);

			// then
			Assert.Equal(2, d.Messages.Count);

			Assert.IsType<CookFood>(d.Messages[0]);
			Assert.IsType<Callback>(d.Messages[1]);

			var x0 = d.Messages[0] as CookFood;
			var x1 = d.Messages[1] as Callback;

			Assert.Equal(o.OrderId, x0.Order.OrderId);
			Assert.Equal(inputEvent.CorrelationId, x0.CorrelationId);
			Assert.Equal(inputEvent.MessageId, x0.ParentId);

			Assert.Equal(inputEvent.CorrelationId, x1.CorrelationId);
			Assert.Equal(inputEvent.MessageId, x1.ParentId);
			Assert.Equal(5, x1.Seconds);
			Assert.IsType<CookFoodCalledBack>(x1.Payload);
		}

		[Fact]
		public void after_first_attempt_succeeded_on_callback_do_nothing() {
			// given
			var d = new FakeDispatcher();
			var m = new Runner(d);
			var o = new Order {OrderId = Guid.NewGuid()};

			m.Handle(new OrderPlaced {Order = o});
			m.Handle(new FoodCooked {Order = o});
			var cookFoodCallback = (d.Messages[1] as Callback).Payload as CookFoodCalledBack;
			d.Clear();

			// when
			m.Handle(cookFoodCallback);

			// then
			Assert.Empty(d.Messages);
		}


		[Fact]
		public void after_foodCooked_then_priceOrder() {
			// given
			var d = new FakeDispatcher();
			var m = new Runner(d);
			var o = new Order {OrderId = Guid.NewGuid()};

			// when
			var inputEvent = new FoodCooked {Order = o};
			m.Handle(inputEvent);

			// then
			Assert.Single(d.Messages);

			Assert.IsType<PriceOrder>(d.Messages[0]);

			var x0 = d.Messages[0] as PriceOrder;

			Assert.Equal(o.OrderId, x0.Order.OrderId);
			Assert.Equal(inputEvent.CorrelationId, x0.CorrelationId);
			Assert.Equal(inputEvent.MessageId, x0.ParentId);
		}

		[Fact]
		public void after_orderPaid_then_orderCompleted() {
			// given
			var d = new FakeDispatcher();
			var m = new Runner(d);
			var o = new Order {OrderId = Guid.NewGuid()};

			// when
			var inputEvent = new OrderPaid {Order = o};
			m.Handle(inputEvent);

			// then
			Assert.Equal(2, d.Messages.Count);

			Assert.IsType<PrintOrder>(d.Messages[0]);
			Assert.IsType<OrderCompleted>(d.Messages[1]);

			var x0 = d.Messages[0] as PrintOrder;
			var x1 = d.Messages[1] as OrderCompleted;

			Assert.Equal(o.OrderId, x0.Order.OrderId);
			Assert.Equal(inputEvent.CorrelationId, x0.CorrelationId);
			Assert.Equal(inputEvent.MessageId, x0.ParentId);

			Assert.True(x1.Success);
			Assert.False(x1.Retried);
			Assert.Equal(o.OrderId, x1.Order.OrderId);
			Assert.Equal(inputEvent.CorrelationId, x1.CorrelationId);
			Assert.Equal(inputEvent.MessageId, x1.ParentId);
		}

		[Fact]
		public void after_orderPlaced_then_cookFood_and_set_timeout() {
			// given
			var d = new FakeDispatcher();
			var m = new Runner(d);
			var o = new Order {OrderId = Guid.NewGuid()};

			// when
			var inputEvent = new OrderPlaced {Order = o};
			m.Handle(inputEvent);

			// then
			Assert.Equal(2, d.Messages.Count);

			Assert.IsType<CookFood>(d.Messages[0]);
			Assert.IsType<Callback>(d.Messages[1]);

			var x0 = d.Messages[0] as CookFood;
			var x1 = d.Messages[1] as Callback;

			Assert.Equal(o.OrderId, x0.Order.OrderId);
			Assert.Equal(inputEvent.CorrelationId, x0.CorrelationId);
			Assert.Equal(inputEvent.MessageId, x0.ParentId);

			Assert.Equal(inputEvent.CorrelationId, x1.CorrelationId);
			Assert.Equal(inputEvent.MessageId, x1.ParentId);
			Assert.Equal(5, x1.Seconds);
			Assert.IsType<CookFoodCalledBack>(x1.Payload);
		}

		[Fact]
		public void after_orderPriced_then_takePayment() {
			// given
			var d = new FakeDispatcher();
			var m = new Runner(d);
			var o = new Order {OrderId = Guid.NewGuid()};

			// when
			var inputEvent = new OrderPriced {Order = o};
			m.Handle(inputEvent);

			// then
			Assert.Single(d.Messages);

			Assert.IsType<TakePayment>(d.Messages[0]);

			var x0 = d.Messages[0] as TakePayment;

			Assert.Equal(o.OrderId, x0.Order.OrderId);
			Assert.Equal(inputEvent.CorrelationId, x0.CorrelationId);
			Assert.Equal(inputEvent.MessageId, x0.ParentId);
		}

		[Fact]
		public void after_second_attempt_failed_on_callback_give_up() {
			// given
			var d = new FakeDispatcher();
			var m = new Runner(d);
			var o = new Order {OrderId = Guid.NewGuid()};

			m.Handle(new OrderPlaced {Order = o});
			var cookFoodCallback1 = (d.Messages[1] as Callback).Payload as CookFoodCalledBack;
			d.Clear();

			m.Handle(cookFoodCallback1);
			var cookFoodCallback2 = (d.Messages[1] as Callback).Payload as CookFoodCalledBack;
			d.Clear();

			// when
			var inputEvent = cookFoodCallback2;
			m.Handle(cookFoodCallback2);

			// then
			Assert.Equal(2, d.Messages.Count);

			Assert.IsType<FailedToContactTheKitchen>(d.Messages[0]);
			Assert.IsType<OrderCompleted>(d.Messages[1]);

			var x0 = d.Messages[0] as FailedToContactTheKitchen;
			var x1 = d.Messages[1] as OrderCompleted;

			Assert.Equal(o.OrderId, x0.Order.OrderId);
			Assert.Equal(inputEvent.CorrelationId, x0.CorrelationId);
			Assert.Equal(inputEvent.MessageId, x0.ParentId);

			Assert.False(x1.Success);
			Assert.True(x1.Retried);
			Assert.Equal(o.OrderId, x1.Order.OrderId);
			Assert.Equal(inputEvent.CorrelationId, x1.CorrelationId);
			Assert.Equal(inputEvent.MessageId, x1.ParentId);
		}

		[Fact]
		public void after_second_attempt_succeeded_on_callback_do_nothing() {
			// given
			var d = new FakeDispatcher();
			var m = new Runner(d);
			var o = new Order {OrderId = Guid.NewGuid()};

			m.Handle(new OrderPlaced {Order = o});
			var cookFoodCallback1 = (d.Messages[1] as Callback).Payload as CookFoodCalledBack;
			d.Clear();

			m.Handle(cookFoodCallback1);
			var cookFoodCallback2 = (d.Messages[1] as Callback).Payload as CookFoodCalledBack;

			m.Handle(new FoodCooked {Order = o});
			d.Clear();

			// when
			m.Handle(cookFoodCallback2);

			// then
			Assert.Empty(d.Messages);
		}

		[Fact]
		public void after_second_attempt_succeeded_then_price_order() {
			// given
			var d = new FakeDispatcher();
			var m = new Runner(d);
			var o = new Order {OrderId = Guid.NewGuid()};

			m.Handle(new OrderPlaced {Order = o});
			var cookFoodCallback1 = (d.Messages[1] as Callback).Payload as CookFoodCalledBack;
			d.Clear();

			m.Handle(cookFoodCallback1);
			var cookFoodCallback2 = (d.Messages[1] as Callback).Payload as CookFoodCalledBack;
			d.Clear();


			// when
			var inputEvent = new FoodCooked {Order = o};
			m.Handle(inputEvent);

			// then
			Assert.Single(d.Messages);

			Assert.IsType<PriceOrder>(d.Messages[0]);

			var x0 = d.Messages[0] as PriceOrder;

			Assert.Equal(o.OrderId, x0.Order.OrderId);
			Assert.Equal(inputEvent.CorrelationId, x0.CorrelationId);
			Assert.Equal(inputEvent.MessageId, x0.ParentId);
		}
	}
}
