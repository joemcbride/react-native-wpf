using System.Collections.Generic;
using System.Diagnostics;

namespace ReactNative.Framework
{
    public abstract class BaseEvent
    {
        public long? ViewTag { get; set; }
        public string EventName { get; set; }
        public IDictionary<string, object> Body { get; set; }

        public abstract string ModuleDotMethod { get; }
    }

    public interface IEventDispatcher
    {
        void SendInputEventWithName(string name, IDictionary<string, object> body);
    }

    public class EventDispatcher : IEventDispatcher
    {
        private readonly IReactBridge _bridge;

        public EventDispatcher(IReactBridge bridge)
        {
            _bridge = bridge;
        }

        public void SendInputEventWithName(string name, IDictionary<string, object> body)
        {
            Debug.Assert(body["target"] is long, "Event body dictionary must include a 'target' property containing the React tag");

            name = name.NormalizeInputEventName();
            _bridge.ExecuteJSCall("RCTEventEmitter", "receiveEvent",
                new[] {body["target"], name, body});
        }
    }
}
