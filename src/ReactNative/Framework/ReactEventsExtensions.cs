namespace ReactNative.Framework
{
    public static class ReactEventsExtensions
    {
        public static string NormalizeInputEventName(this string eventName)
        {
            if (eventName.StartsWith("on"))
            {
                eventName = "top" + eventName.Substring(2);
            }
            else if (!eventName.StartsWith("top"))
            {
                eventName = "top" + eventName.Substring(0, 1).ToUpper() + eventName.Substring(1);
            }

            return eventName;
        }
    }
}