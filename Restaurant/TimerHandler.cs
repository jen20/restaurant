using System.Threading;
using System.Threading.Tasks;

namespace Restaurant {
	internal class TimerHandler : IHandle<Callback> {
		private int _count;
		private readonly IPublisher _publisher;

		public TimerHandler(IPublisher publisher) {
			_publisher = publisher;
		}

		public int Count => _count;

		public void Handle(Callback t) {
			Go(t);
		}

		private async Task Go(Callback t) {
			Interlocked.Increment(ref _count);
			await Task.Delay(t.Seconds * 1000);
			try {
				_publisher.Publish(t.Payload);
			} finally {
				Interlocked.Decrement(ref _count);
			}
		}
	}
}
