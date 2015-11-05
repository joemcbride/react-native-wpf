using System;
using System.Collections.Generic;

namespace ReactNative.Framework
{
    public static class ExtensionMethods
    {
        public static void Apply<T>(this IEnumerable<T> items, Action<T, int> action)
        {
            var index = 0;
            foreach (var item in items)
            {
                action(item, index);
                index++;
            }
        }
        public static IEnumerable<K> Apply<T,K>(this IEnumerable<T> items, Func<T, int, K> action)
        {
            var index = 0;
            var results = new List<K>();
            foreach (var item in items)
            {
                var result = action(item, index);
                results.Add(result);
                index++;
            }

            return results;
        }

        public static string ToFormat(this string format, params object[] args)
        {
            return string.Format(format, args);
        }

        public static bool IsReactRootView(this IReactComponent component)
        {
            return component != null && component.ReactTag.HasValue && component.ReactTag.Value%10 == 1;
        }
    }
}
