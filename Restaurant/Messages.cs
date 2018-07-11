using System;

namespace Restaurant {
	public abstract class Message {
		public Guid MessageId = Guid.NewGuid();
		public Guid? ParentId;

		public Message(Message parent = null) {
			if (parent == null)
				return;

			ParentId = parent.MessageId;
			CorrelationId = parent.CorrelationId;
		}

		public Guid CorrelationId { get; } = Guid.NewGuid();
	}

	public abstract class Command : Message {
		public Command(Message parent = null) : base(parent) {
		}
	}

	public abstract class Event : Message {
		public Event(Message parent = null) : base(parent) {
		}
	}

	public class CookFood : Command {
		public Order Order;

		public CookFood(Message parent) : base(parent) {
		}
	}

	public class PriceOrder : Command {
		public Order Order;

		public PriceOrder(Message parent) : base(parent) {
		}
	}

	public class TakePayment : Command {
		public Order Order;

		public TakePayment(Message parent) : base(parent) {
		}
	}

	public class PrintOrder : Command {
		public Order Order;

		public PrintOrder(Message parent) : base(parent) {
		}
	}

	public class Callback : Command {
		public Message Payload;
		public int Seconds;

		public Callback(Message parent = null) : base(parent) {
		}
	}

	public class OrderPlaced : Event {
		public Order Order;
	}

	public class FoodCooked : Event {
		public Order Order;

		public FoodCooked(Message parent = null) : base(parent) {
		}
	}

	public class OrderPriced : Event {
		public Order Order;

		public OrderPriced(Message parent = null) : base(parent) {
		}
	}

	public class OrderPaid : Event {
		public Order Order;

		public OrderPaid(Message parent = null) : base(parent) {
		}
	}

	public class OrderCompleted : Event {
		public Order Order;
		public bool Retried;
		public bool Success;

		public OrderCompleted(Message parent) : base(parent) {
		}
	}

	public class CookFoodCalledBack : Event {
		public int Attempt;
		public Order Order;

		public CookFoodCalledBack(Message parent) : base(parent) {
		}
	}

	public class FailedToContactTheKitchen : Event {
		public Order Order;

		public FailedToContactTheKitchen(Message parent) : base(parent) {
		}
	}
}
