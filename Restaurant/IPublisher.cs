namespace Restaurant {
	public interface IPublisher {
		void Publish<T>(T t) where T : Message;
	}
}
