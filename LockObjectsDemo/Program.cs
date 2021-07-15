using System;
using System.Threading.Tasks;

namespace LockObjectsDemo
{
	class Program
	{
		static async Task Main(string[] args)
		{
			await ConcurrencyDemo();
		}

		static async Task ConcurrencyDemo()
		{
			var service = new MyService();

			Console.WriteLine("[WITH LOCK] 4 requests for the same ticket \"MyUniqueId\"");

			var task1 = service.LockAndStoreUniqueValue(1200, "Person1", "TicketId1");
			var task2 = service.LockAndStoreUniqueValue(800, "Person2", "TicketId1");
			var task3 = service.LockAndStoreUniqueValue(500, "Person3", "TicketId1");
			var task4 = service.LockAndStoreUniqueValue(100, "Person4", "TicketId1");

			Task.WaitAll(new Task[] { task1, task2, task3, task4 });

			Console.WriteLine("\nResults:");

			foreach (var item in MyService.FakeRepository)
			{
				Console.WriteLine(item);
			}

			Console.WriteLine("\nEmptying repository\n");
			service.EmptyRepository();

			Console.WriteLine("[WITHOUT LOCK] 4 requests for the same ticket \"MyUniqueId\"");

			var task5 = service.DoNotLockAndStoreUniqueValue(1200, "Person1", "TicketId1");
			var task6 = service.DoNotLockAndStoreUniqueValue(800, "Person2", "TicketId1");
			var task7 = service.DoNotLockAndStoreUniqueValue(500, "Person3", "TicketId1");
			var task8 = service.DoNotLockAndStoreUniqueValue(100, "Person4", "TicketId1");

			Task.WaitAll(new Task[] { task5, task6, task7, task8 });

			Console.WriteLine("\nResults:");

			foreach (var item in MyService.FakeRepository)
			{
				Console.WriteLine(item);
			}
		}
	}
}
