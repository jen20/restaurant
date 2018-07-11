using System;

namespace Restaurant {
	public class ScrewUp<T> : IHandle<T> where T : Message {
		private readonly IHandle<T> _inner;
		private readonly Random _rand = new Random(1);

		public ScrewUp(IHandle<T> inner) {
			_inner = inner;
		}

		public void Handle(T t) {
			if (_rand.Next(3) == 0)
				return;
			_inner.Handle(t);

			if (_rand.Next(3) == 0)
				_inner.Handle(t);
		}
	}
}
