namespace Restaurant {
	public class Runner : IRunner {
		private bool _calledback;
		private bool _cooked;
		private readonly IPublisher _dispatcher;

		public Runner(IPublisher dispatcher) {
			_dispatcher = dispatcher;
		}

		public void Handle(OrderPlaced m) {
			_dispatcher.Publish(new CookFood(m) {Order = m.Order});
			_dispatcher.Publish(new Callback(m) {
				Seconds = 2,
				Payload = new CookFoodCalledBack(m) {
					Attempt = 1,
					Order = m.Order
				}
			});
		}

		public void Handle(CookFoodCalledBack m) {
			Apply(m);
			HandleFirstAttemptCallback(m);
			HandleSecondAttemptCallback(m);
		}

		public void Handle(FoodCooked m) {
			Apply(m);
			_dispatcher.Publish(new PriceOrder(m) {Order = m.Order});
		}

		public void Handle(OrderPriced m) {
			_dispatcher.Publish(new TakePayment(m) {Order = m.Order});
		}

		public void Handle(OrderPaid m) {
			_dispatcher.Publish(new PrintOrder(m) {Order = m.Order});
			_dispatcher.Publish(new OrderCompleted(m) {
				Order = m.Order,
				Success = true,
				Retried = _calledback
			});
		}

		private void HandleFirstAttemptCallback(CookFoodCalledBack m) {
			if (m.Attempt != 1 || _cooked)
				return;

			_dispatcher.Publish(new CookFood(m) {Order = m.Order});
			_dispatcher.Publish(new Callback(m) {
				Seconds = 2,
				Payload = new CookFoodCalledBack(m) {
					Attempt = 2,
					Order = m.Order
				}
			});
		}

		private void HandleSecondAttemptCallback(CookFoodCalledBack m) {
			if (m.Attempt != 2 || _cooked)
				return;

			_dispatcher.Publish(new FailedToContactTheKitchen(m) {Order = m.Order});
			_dispatcher.Publish(new OrderCompleted(m) {
				Order = m.Order,
				Success = false,
				Retried = true
			});
		}

		public void Apply(CookFoodCalledBack e) {
			_calledback = true;
		}

		private void Apply(FoodCooked e) {
			_cooked = true;
		}
	}
}
