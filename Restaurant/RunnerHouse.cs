using System;
using System.Collections.Generic;

namespace Restaurant {
	internal interface IRunner :
		IHandle<OrderPlaced>,
		IHandle<FoodCooked>,
		IHandle<CookFoodCalledBack>,
		IHandle<OrderPriced>,
		IHandle<OrderPaid> {
	}

	public class RunnerHouse : IHandle<Event> {
		private readonly Dictionary<Guid, IRunner> _dict = new Dictionary<Guid, IRunner>();
		private readonly Dispatcher _dispatcher;

		public RunnerHouse(Dispatcher d) {
			_dispatcher = d;
		}

		public int CountRunners => _dict.Count;

		public void Handle(Event m) {
			HandleImpl((dynamic)m);
		}

		public void HandleImpl(Event m) {
		}

		private void HandleImpl(OrderPlaced t) {
			var m = t.Order.IsDodge
				? (IRunner)new PayFirstRunner(_dispatcher)
				: new Runner(_dispatcher);
			_dict[t.CorrelationId] = m;

			m.Handle(t);
		}

		private void HandleImpl(FoodCooked t) {
			if (_dict.TryGetValue(t.CorrelationId, out var m)) m.Handle(t);
		}

		private void HandleImpl(CookFoodCalledBack t) {
			if (_dict.TryGetValue(t.CorrelationId, out var m)) m.Handle(t);
		}

		private void HandleImpl(OrderPriced t) {
			if (_dict.TryGetValue(t.CorrelationId, out var m)) m.Handle(t);
		}

		private void HandleImpl(OrderPaid t) {
			if (_dict.TryGetValue(t.CorrelationId, out var m)) m.Handle(t);
		}

		private void HandleImpl(OrderCompleted t) {
			_dict.Remove(t.CorrelationId);
		}
	}
}
