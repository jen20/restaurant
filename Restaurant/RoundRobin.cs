using System.Collections.Generic;
using System.Linq;

namespace Restaurant {
	public class RoundRobin<T> : IHandle<T> where T : Message {
		private readonly List<IHandle<T>> _handlers;
		private int _nextHandler;

		public RoundRobin(IEnumerable<IHandle<T>> handlers) {
			_handlers = handlers.ToList();
		}

		public void Handle(T t) {
			_handlers[_nextHandler].Handle(t);
			_nextHandler = (_nextHandler + 1) % _handlers.Count;
		}
	}
}
