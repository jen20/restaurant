using System.Collections.Generic;
using System.Linq;

namespace Restaurant {
	public class Multiplexer<T> : IHandle<T> where T : Message {
		private readonly List<IHandle<T>> _handlers;

		public Multiplexer(IEnumerable<IHandle<T>> handlers) {
			_handlers = handlers.ToList();
		}

		public void Handle(T t) {
			foreach (var handler in _handlers)
				handler.Handle(t);
		}
	}
}
