using System;

namespace Restaurant {
	public class Handler<T> : IHandle<T> where T : Message {
		private readonly Action<T> _action;

		public Handler(Action<T> action) {
			_action = action;
		}

		public void Handle(T t) {
			_action(t);
		}
	}
}
