using System;
using System.Threading;
using System.Threading.Tasks;

namespace LockObjectsDemo
{
	class Program
	{
		static async Task Main(string[] args)
		{
			await ConcurrencyDemo();
			await SynchronizeDemo();
		}

		static async Task ConcurrencyDemo()
		{
			Console.WriteLine("\nConcurrencyDemo:\n");

			var service = new MyService();

			Console.WriteLine("[WITH LOCK] 4 requests for the same ticket \"TicketId1\"");

			var task1 = service.LockAndStoreUniqueValue(1200, "Person1", "TicketId1");
			var task2 = service.LockAndStoreUniqueValue(800, "Person2", "TicketId1");
			var task3 = service.LockAndStoreUniqueValue(500, "Person3", "TicketId1");
			var task4 = service.LockAndStoreUniqueValue(100, "Person4", "TicketId1");

			await Task.WhenAll(new Task[] { task1, task2, task3, task4 });

			Console.WriteLine("\nResults:");

			foreach (var item in MyService.FakeRepository)
			{
				Console.WriteLine(item);
			}

			Console.WriteLine("\nEmptying repository\n");
			service.EmptyRepository();

			Console.WriteLine("[WITHOUT LOCK] 4 requests for the same ticket \"TicketId1\"");

			var task5 = service.DoNotLockAndStoreUniqueValue(1200, "Person1", "TicketId1");
			var task6 = service.DoNotLockAndStoreUniqueValue(800, "Person2", "TicketId1");
			var task7 = service.DoNotLockAndStoreUniqueValue(500, "Person3", "TicketId1");
			var task8 = service.DoNotLockAndStoreUniqueValue(100, "Person4", "TicketId1");

			await Task.WhenAll(new Task[] { task5, task6, task7, task8 });

			Console.WriteLine("\nResults:");

			foreach (var item in MyService.FakeRepository)
			{
				Console.WriteLine(item);
			}
		}

		/// <summary>
		/// Values 1 and 2 is grouped on the same key and A is grouped with B
		/// 1 will always return before 2 and A will always return before B even though they 1 and A takes the longest to run
		/// If we were to add 3 and C these would compete for the second place since then we would have 2 threads waiting to enter the same lock at the same time.
		/// It works with 2 calls for each key becuase we can make sure the first in each key will be called before the second.
		/// </summary>
		/// <returns></returns>
		static async Task SynchronizeDemo()
		{
			Console.WriteLine("\nSynchronizeDemo:\n");

			var service = new MyService();

			var task1 = service.SleepAndReturnInput(500, "1", "key1")
				.ContinueWith(async (task) => { Console.WriteLine(await task); });
			Thread.Sleep(10); //The sleep makes sure the functions starts to execute in the right order for the sake of the demo.

			var task2 = service.SleepAndReturnInput(250, "2", "key1")
				.ContinueWith(async (task) => { Console.WriteLine(await task); });

			var task3 = service.SleepAndReturnInput(600, "A", "key2")
				.ContinueWith(async (task) => { Console.WriteLine(await task); });
			Thread.Sleep(10);

			var task4 = service.SleepAndReturnInput(100, "B", "key2")
				.ContinueWith(async (task) => { Console.WriteLine(await task); });
			
			Task.WaitAll(new Task[] { task1, task2, task3, task4});
		}
	}
}
