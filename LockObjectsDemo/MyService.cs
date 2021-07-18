using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LockObjectsDemo
{
	public class MyService
	{
		public static List<KeyValuePair<string, string>> FakeRepository = new List<KeyValuePair<string, string>>();

		public async Task LockAndStoreUniqueValue(int sleep, string value, string keyword)
		{
			await Task.Run(() =>
			{
				KeywordLocker.WrapInLock(() => StoreUniqueValue(sleep, value, keyword), keyword);
			});
		}

		public async Task DoNotLockAndStoreUniqueValue(int sleep, string value, string keyword)
		{
			await Task.Run(() =>
			{
				StoreUniqueValue(sleep, value, keyword);
			});
		}

		public void EmptyRepository()
		{
			FakeRepository = new List<KeyValuePair<string, string>>();
		}

		private void StoreUniqueValue(int sleep, string value, string keyword)
		{
			if (!FakeRepository.Any(x => x.Key == keyword))
			{
				Thread.Sleep(sleep);
				FakeRepository.Add(new KeyValuePair<string, string>(keyword, value));
			}
		}

		public async Task<string> SleepAndReturnInput(int sleep, string input, string keyword)
		{
			return await Task.Run(() =>
			{
				return KeywordLocker.WrapInLock(() => SleepAndReturnInput(sleep, input), keyword);
			});


			//return KeyWordLocker.WrapInLock(() => SleepAndReturnInput(sleep, input), keyword);

			//var lockObject = LockList.GetOrAdd(keyword, new object());
			//lock (lockObject)
			//{
			//	try
			//	{
			//		return Task.Run(() =>
			//		{
			//			Thread.Sleep(sleep);
			//			return input;
			//		});
			//	}
			//	finally
			//	{
			//		LockList.TryRemove(keyword, out _);
			//	}
			//}

			//KeywordLocker.LockAction(keyword, () => { await 
			//	SleepAndReturnInput(sleep, input)});

		}

		private string SleepAndReturnInput(int sleep, string input)
		{
			Thread.Sleep(sleep);
			return input;
		}
	}
}
