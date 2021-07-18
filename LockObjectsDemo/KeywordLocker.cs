using System;
using System.Collections.Concurrent;

namespace LockObjectsDemo
{
	public static class KeywordLocker
	{
		private readonly static ConcurrentDictionary<string, object> _lockList = new ConcurrentDictionary<string, object>();
		public static TResult WrapInLock<TResult>(Func<TResult> function, string keyword)
		{
			var lockObject = _lockList.GetOrAdd(keyword, new object());
			lock (lockObject)
			{
				try
				{
					return function();
				}
				finally
				{
					_lockList.TryRemove(keyword, out _);
				}
			}
		}

		public static void WrapInLock(Action function, string keyword)
		{
			var lockObject = _lockList.GetOrAdd(keyword, new object());
			lock (lockObject)
			{
				try
				{
					function();
				}
				finally
				{
					_lockList.TryRemove(keyword, out _);
				}
			}
		}
	}
}
