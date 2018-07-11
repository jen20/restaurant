using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Restaurant {
	// This is how competing consumers work in eventstore.
	// You tune the size of the buffer (in this case 5)
	// according to the normal speed of the message processing
	// to avoid idle time at any particular handler.
	public class MorefairDispatcher<T> : IHandle<T> where T : Message {
		private readonly List<ThreadedHandler<T>> _handlers;

		public MorefairDispatcher(IEnumerable<ThreadedHandler<T>> handlers) {
			_handlers = handlers.ToList();
		}

		public void Handle(T t) {
			while (true) {
				foreach (var handler in _handlers)
					if (handler.Count < 5) {
						handler.Handle(t);
						return;
					}

				Thread.Sleep(1);
			}
		}
	}
}
