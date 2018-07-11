namespace Restaurant
{
	public interface IHandle<T> where T : Message {
		void Handle(T t);
	}
}