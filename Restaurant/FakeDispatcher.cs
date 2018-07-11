using System.Collections.Generic;

namespace Restaurant {
	public class FakeDispatcher : IPublisher {
		public List<Message> Messages = new List<Message>();

		public void Publish<T>(T t) where T : Message {
			Messages.Add(t);
		}

		public void Clear() {
			Messages.Clear();
		}
	}
}
