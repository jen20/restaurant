using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Restaurant {
	// Really useful for linearising requests from multiple threads into one
	// as per 3 cooks running in different thread handlers passing orders on to a
	// single assman.
	//
	// BUIlding up the pipeline in this way is called SEDA.
	//
	// Each piece is single threaded and we determine the threading model by how we plug them together
	// with ThreadedHandlers.

	// in the threadedhandler add a count
	// in the timer print out the sizes: name an count

	public class ThreadedHandler<T> : IHandle<T> where T : Message {
		private readonly IHandle<T> _handler;
		private readonly ConcurrentQueue<T> _queue = new ConcurrentQueue<T>();
		private readonly Thread _thread;

		public ThreadedHandler(string name, IHandle<T> handler) {
			_handler = handler;
			_thread = new Thread(DoWork);
			Name = name;
		}

		public string Name { get; }

		public int Count => _queue.Count;

		public void Handle(T t) {
			_queue.Enqueue(t);
		}

		public void Start() {
			_thread.Start();
		}

		public void DoWork() {
			while (true)
				try {
					if (_queue.TryDequeue(out var o))
						_handler.Handle(o);
					else
						Thread.Sleep(1);
				} catch (Exception e) {
					Console.WriteLine($"Woops {e}");
				}
		}
	}
}
