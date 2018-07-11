using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Restaurant {
	public class Cook : IHandle<CookFood> {
		private readonly ConcurrentDictionary<Guid, bool> _cookedOrders;
		private readonly int _cookingTime;
		private readonly IPublisher _publisher;

		public Cook(
			string name,
			int cookingTime,
			ConcurrentDictionary<Guid, bool> cookedOrders,
			IPublisher publisher) {
			Name = name;
			_cookingTime = cookingTime;
			_cookedOrders = cookedOrders;
			_publisher = publisher;
		}

		public string Name { get; }

		public void Handle(CookFood e) {
			if (!_cookedOrders.TryAdd(e.Order.OrderId, true))
				return;

			Thread.Sleep(_cookingTime);

			e.Order.CookedBy = Name;
			e.Order.CookingTime = e.Order.Items.Count * 2;

			_publisher.Publish(new FoodCooked(e) {Order = e.Order});
		}
	}
}
