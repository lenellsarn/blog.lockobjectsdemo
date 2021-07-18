using System;
using System.Collections.Generic;
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

			Console.WriteLine("[WITHOUT LOCK] 8 simultanous requests for 2 tickets \"TicketId1\" and \"TicketId2\"");

			Random random = new Random();

			var tasks = new List<Task>();
			tasks.Add(service.DoNotLockAndStoreUniqueValue(random.Next(100, 1200), "Olle", "TicketId1"));
			tasks.Add(service.DoNotLockAndStoreUniqueValue(random.Next(100, 1200), "Anna", "TicketId1"));
			tasks.Add(service.DoNotLockAndStoreUniqueValue(random.Next(100, 1200), "Pelle", "TicketId1"));
			tasks.Add(service.DoNotLockAndStoreUniqueValue(random.Next(100, 1200), "Greta", "TicketId1"));
			tasks.Add(service.DoNotLockAndStoreUniqueValue(random.Next(100, 1200), "Göran", "TicketId2"));
			tasks.Add(service.DoNotLockAndStoreUniqueValue(random.Next(100, 1200), "Karin", "TicketId2"));
			tasks.Add(service.DoNotLockAndStoreUniqueValue(random.Next(100, 1200), "Anders", "TicketId2"));
			tasks.Add(service.DoNotLockAndStoreUniqueValue(random.Next(100, 1200), "Lina", "TicketId2"));

			await Task.WhenAll(tasks);

			Console.WriteLine("\nResults:");

			foreach (var item in MyService.FakeRepository)
			{
				Console.WriteLine(item);
			}

			Console.WriteLine("\nEmptying repository\n");
			service.EmptyRepository();

			Console.WriteLine("[WITH LOCK] 8 simultanous requests for 2 tickets \"TicketId1\" and \"TicketId2\"");

			tasks = new List<Task>();
			tasks.Add(service.LockAndStoreUniqueValue(random.Next(100, 1200), "Olle", "TicketId1"));
			tasks.Add(service.LockAndStoreUniqueValue(random.Next(100, 1200), "Anna", "TicketId1"));
			tasks.Add(service.LockAndStoreUniqueValue(random.Next(100, 1200), "Pelle", "TicketId1"));
			tasks.Add(service.LockAndStoreUniqueValue(random.Next(100, 1200), "Greta", "TicketId1"));
			tasks.Add(service.LockAndStoreUniqueValue(random.Next(100, 1200), "Göran", "TicketId2"));
			tasks.Add(service.LockAndStoreUniqueValue(random.Next(100, 1200), "Karin", "TicketId2"));
			tasks.Add(service.LockAndStoreUniqueValue(random.Next(100, 1200), "Anders", "TicketId2"));
			tasks.Add(service.LockAndStoreUniqueValue(random.Next(100, 1200), "Lina", "TicketId2"));

			await Task.WhenAll(tasks);

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
