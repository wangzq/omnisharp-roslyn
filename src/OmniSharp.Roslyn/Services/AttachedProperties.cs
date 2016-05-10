using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace OmniSharp.Services
{
	public static class AttachedProperties
	{
		// using ConditionalWeakTable can make sure the dictionary attached to the object can also
		// be released (garbage collected) if the key object is released
		private static readonly ConditionalWeakTable<object, ConcurrentDictionary<string, object>> properties =
			new ConditionalWeakTable<object, ConcurrentDictionary<string, object>>();

		public static T SetProperty<T>(this T obj, string name, object value)
		{
			var props = properties.GetOrCreateValue(obj);
			props.AddOrUpdate(name, _ => value, (_, __) => value);
			return obj;
		}

		public static object GetProperty(this object obj, string name)
		{
			ConcurrentDictionary<string, object> props;
			if (properties.TryGetValue(obj, out props))
			{
				object value;
				props.TryGetValue(name, out value);
				return value;
			}
			
			return null;
		}
	}
}
