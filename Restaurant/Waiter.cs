using System;
using System.Collections.Generic;

namespace Restaurant {
	public class Waiter {
		private readonly IPublisher _publisher;
		private readonly Random _random = new Random(1);

		public Waiter(IPublisher publisher) {
			_publisher = publisher;
		}

		public Guid PlaceOrder(
			string waiter,
			int tableNumber,
			List<Tuple<string, int>> items) {
			var o = new Order {
				OrderId = Guid.NewGuid(),
				Waiter = waiter,
				TableNumber = tableNumber,
				IsDodge = _random.Next(2) == 1
			};

			foreach (var item in items)
				o.Items.Add(new Order.Item {
					Name = item.Item1,
					Quantity = item.Item2
				});

			_publisher.Publish(new OrderPlaced {Order = o});

			return o.OrderId;
		}
	}
}
