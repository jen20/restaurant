using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

/*

    Waiter, Cook, Assistant Manager, Cashier

    Waiter takes orders
    Takes orders to cooks queue
    Cook knows how to cook the food from a database of recepies.
    Cook enriches the order with the food ingredients and time to cook
    When the cook finishes the food he rings the bell and puts the order on the counter.
    Assistant manager has a database of prices, and riches the order with prices, taxes, totals
    Assistant manager brings the order to the cashier.
    Cashier takes payment and puts the order on the spike
    Order goes into reporting database

    We pass a document from person to person.
    We must be able to handle a document and create an adjusted copy
    of it without losing data from it that isn't in our schema

    The waiter is a messaging pattern called an originator.

*/
namespace Restaurant {
	internal static class Program {
		private static void Main(string[] args) {
			var dispatcher = new Dispatcher();

			var waiter = new Waiter(dispatcher);
			var assistant = new AssistantManager(dispatcher);
			var cashier = new Cashier(dispatcher);

			var timer = new TimerHandler(dispatcher);
			var house = new RunnerHouse(dispatcher);
			var serialHouse =
				new ThreadedHandler<Event>(
					"House",
					house);

			// set up the kitchen
			var cookedOrders = new ConcurrentDictionary<Guid, bool>();
			var cooks = new List<Cook> {
				new Cook("James", 100, cookedOrders, dispatcher),
				new Cook("Emily", 50, cookedOrders, dispatcher),
				new Cook("Sarah", 500, cookedOrders, dispatcher)
			};

			var threadedCooks = cooks.Select(c =>
				new ThreadedHandler<CookFood>(c.Name, c)).ToList();

			var kitchen =
				new ThreadedHandler<CookFood>(
					"Kitchen",
					new ScrewUp<CookFood>(
						new MorefairDispatcher<CookFood>(
							threadedCooks)));

			var numCompleted = 0;
			var numFailedImmediately = 0;
			var numFailedAfterRetry = 0;
			var numSucceededImmediately = 0;
			var numSucceededAfterRetry = 0;

			var onCompleted =
				new ThreadedHandler<OrderCompleted>(
					"OnCompleted",
					new Handler<OrderCompleted>(e => {
						numCompleted++;
						if (e.Success && e.Retried) numSucceededAfterRetry++;
						if (e.Success && !e.Retried) numSucceededImmediately++;
						if (!e.Success && e.Retried) numFailedAfterRetry++;
						if (!e.Success && !e.Retried) numFailedImmediately++;
					}));

			// subscribe everything
			dispatcher.Subscribe(kitchen);
			dispatcher.Subscribe(assistant);
			dispatcher.Subscribe(cashier);
			dispatcher.Subscribe(serialHouse);
			dispatcher.Subscribe(timer);
			dispatcher.Subscribe(onCompleted);

			// subscribe various printers
			//dispatcher.Subscribe(new Handler<CookFood>(e => Console.WriteLine($"{e.CorrelationId} Command: CookFood")));
			//dispatcher.Subscribe(new Handler<PriceOrder>(e => Console.WriteLine($"{e.CorrelationId} Command: PriceOrder")));
			//dispatcher.Subscribe(new Handler<TakePayment>(e => Console.WriteLine($"{e.CorrelationId} Command: TakePayment")));
			//dispatcher.Subscribe(new Handler<PrintOrder>(e => Console.WriteLine($"{e.CorrelationId} Command: PrintOrder")));
			//dispatcher.Subscribe(new Handler<Callback>(e => Console.WriteLine($"{e.CorrelationId} Command: Callback")));
			//dispatcher.Subscribe(new Handler<OrderPlaced>(e => Console.WriteLine($"{e.CorrelationId} Ordered")));
			//dispatcher.Subscribe(new Handler<FoodCooked>(e => Console.WriteLine($"{e.CorrelationId} Cooked")));
			//dispatcher.Subscribe(new Handler<OrderPriced>(e => Console.WriteLine($"{e.CorrelationId} Priced")));
			//dispatcher.Subscribe(new Handler<OrderPaid>(e => Console.WriteLine($"{e.CorrelationId} Paid")));
			//dispatcher.Subscribe(new Handler<OrderCompleted>(e => Console.WriteLine($"{e.CorrelationId} OrderCompleted")));
			//dispatcher.Subscribe(new Handler<CookFoodCalledBack>(e => Console.WriteLine($"{e.CorrelationId} CookFoodCalledBack")));
			//dispatcher.Subscribe(new Handler<FailedToContactTheKitchen>(e => Console.WriteLine($"{e.CorrelationId} Failed")));

			// start everything
			serialHouse.Start();
			kitchen.Start();
			foreach (var cook in threadedCooks)
				cook.Start();
			onCompleted.Start();

			var numOrders = 300;
			for (var i = 0; i < numOrders; i++)
				waiter.PlaceOrder("Toby", 1, new List<Tuple<string, int>> {
					Tuple.Create("Meat", 2),
					Tuple.Create("Veg", 1)
				});

			Action print = () => {
				Console.WriteLine($"-----------------------------------------");
				Console.WriteLine($"{kitchen.Name}: {kitchen.Count}");
				foreach (var cook in threadedCooks)
					Console.WriteLine($"{cook.Name}: {cook.Count}");
				Console.WriteLine($"{onCompleted.Name}: {onCompleted.Count}");

				Console.WriteLine($"Timer {timer.Count}");
				Console.WriteLine($"Runners {house.CountRunners}");
				Console.WriteLine($"num Failed Immediately {numFailedImmediately}");
				Console.WriteLine($"num Failed AfterRetry {numFailedAfterRetry}");
				Console.WriteLine($"num Succeeded Immediately {numSucceededImmediately}");
				Console.WriteLine($"num Succeeded AfterRetry {numSucceededAfterRetry}");
				Console.WriteLine($"Complete {numCompleted}/{numOrders}");

				Thread.Sleep(1000);
			};

			while (numCompleted < numOrders)
				print();
			print();
			Console.WriteLine("All done");
		}

		/*
	     * waiter will decide if cusomter is dodgy.
	     * if so pay first
	     * otherwise cook first
	     * we are going to make a Runner who consumes all the events
	     * and produces the appropraite commands
	     * the runner is the process manager! the process can change, we just change the process manager
	     * or create a new process manager.
	     *
	     *  [*] unit test runner
	     *  [*] add IsDodge
	     *  [*] add a OrderCompleted event upon which the runner is removed from the dictionary. <-- wasn't mentioned in class
	     *  [*] use factory to have 2 runners, one for each business process
	     *  [*] add handler 'screw things up' : Handle<T> which wraps a handle<t> and it will randomly drop, or, later, repeat the messages.
	     *  [*] wrap the cooks with screwthings up.
	     *  [*] now the runner will get stuck sometimes.
	     *  [*] detect when a cook t imes out and send the order again. if it fails twice the ngive up and raise a 'ive failed' event.
	     *  [*] make the cooks idempotent. via a shared dictionary.
	     *
	     *  idempotency and the timeouts are the two main ways of handling error scenarios.
	     */
	}
}
