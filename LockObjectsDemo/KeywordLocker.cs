using System;
using System.Collections.Concurrent;

namespace LockObjectsDemo
{
	public static class KeywordLocker
	{
		private readonly static ConcurrentDictionary<string, object> LockList = new ConcurrentDictionary<string, object>();
		public static TResult WrapInLock<TResult>(Func<TResult> function, string keyword)
		{
			var lockObject = LockList.GetOrAdd(keyword, new object());
			lock (lockObject)
			{
				try
				{
					return function();
				}
				finally
				{
					LockList.TryRemove(keyword, out _);
				}
			}
		}

		public static void WrapInLock(Action function, string keyword)
		{
			var lockObject = LockList.GetOrAdd(keyword, new object());
			lock (lockObject)
			{
				try
				{
					function();
				}
				finally
				{
					LockList.TryRemove(keyword, out _);
				}
			}
		}
	}
}
