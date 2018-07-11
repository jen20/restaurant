using System;
using System.Collections.Generic;

namespace Restaurant {
	public class Dispatcher : IPublisher {
		private readonly Dictionary<string, List<Action<Message>>> _handlers =
			new Dictionary<string, List<Action<Message>>>();

		public void Publish<T>(T t) where T : Message {
			var m = t as Message;

			var type = t.GetType();
			while (type != null) {
				PublishInternal(type.FullName, m);
				type = type.BaseType;
			}

			PublishInternal($"{t.CorrelationId}", m);
		}

		private void PublishInternal(string topic, Message m) {
			if (_handlers.TryGetValue(topic, out var h2))
				foreach (var handler in h2)
					handler(m);
		}

		public void Subscribe<T>(IHandle<T> handler) where T : Message {
			FindOrCreateHandlers(typeof(T).FullName).Add(m =>
				handler.Handle((T)m));
		}

		public void Subscribe<T>(Guid correlationId, IHandle<T> handler) where T : Message {
			FindOrCreateHandlers($"{correlationId}").Add(m => {
				if (m is T t)
					handler.Handle(t);
			});
		}

		private List<Action<Message>> FindOrCreateHandlers(string topic) {
			if (!_handlers.TryGetValue(topic, out var handlers)) {
				handlers = new List<Action<Message>>();
				_handlers[topic] = handlers;
			}

			return handlers;
		}
	}
}
