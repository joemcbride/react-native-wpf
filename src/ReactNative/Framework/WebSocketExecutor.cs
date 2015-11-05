using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebSocketSharp;

namespace ReactNative.Framework
{
    public class WebSocketExecutor : IJavaScriptExecutor, IDisposable
    {
        public static int Port = 3333;

        private static long LastID = 100;

        private WebSocket _webSocket;
        private readonly Dictionary<string, object> _injectables = new Dictionary<string, object>();
        private readonly Dictionary<long, Action<string>> _callbacks = new Dictionary<long, Action<string>>();

        public Task<string> Execute(string script, string sourceUrl)
        {
            System.Diagnostics.Debug.WriteLine("Executing Script: {0}".ToFormat(sourceUrl));
            var tcs = new TaskCompletionSource<string>();
            SendMessage(new Dictionary<string, object>
            {
                {"method", "executeApplicationScript"},
                {"url", sourceUrl},
                {"inject", _injectables}
            }, res => tcs.TrySetResult(res));
            return tcs.Task;
        }

        public Task<string> ExecuteJSCall(string name, string method, object[] arguments)
        {
//            System.Diagnostics.Debug.WriteLine("Executing JSCall: {0}.{1}".ToFormat(name, method));
            var tcs = new TaskCompletionSource<string>();
            SendMessage(new Dictionary<string, object>
            {
                {"method", "executeJSCall"},
                {"moduleName", name},
                {"moduleMethod", method},
                {"arguments", arguments}
            }, res => tcs.TrySetResult(res));
            return tcs.Task;
        }

        public Task InjectJSON(string objectName, string json)
        {
            _injectables[objectName] = json;
            var tcs = new TaskCompletionSource<bool>();
            tcs.TrySetResult(true);
            return tcs.Task;
        }

        public void Setup()
        {
            if (_webSocket == null)
            {
                Connect().Wait();
            }
            else
            {
                PrepareJSRuntime().Wait();
            }
        }

        public Task<bool> Connect()
        {
            var tcs = new TaskCompletionSource<bool>();

            _webSocket = new WebSocket("ws://127.0.0.1:{0}/debugger-proxy".ToFormat(Port));
            _webSocket.OnOpen += async (sender, args) =>
            {
                System.Diagnostics.Debug.WriteLine("WS: Opened");
                await PrepareJSRuntime();
                tcs.TrySetResult(true);
            };
            _webSocket.OnClose += (sender, args) =>
            {
                System.Diagnostics.Debug.WriteLine("WS: Closed");
            };
            _webSocket.OnMessage += (sender, args) =>
            {
                HandleJson(args.Data);
            };
            _webSocket.ConnectAsync();

            return tcs.Task;
        }

        public Task<bool> PrepareJSRuntime()
        {
            var tcs = new TaskCompletionSource<bool>();
            SendMessage(new Dictionary<string, object> { {"method", "prepareJSRuntime" } }, res =>
            {
                System.Diagnostics.Debug.WriteLine("WS: Prepare Reply");
                tcs.TrySetResult(true);
            });

            return tcs.Task;
        }

        public void SendMessage(Dictionary<string, object> message, Action<string> callback = null)
        {
            var id = LastID++;
            message["id"] = id;

            _callbacks[id] = callback;

            var json = JsonConvert.SerializeObject(message);
            _webSocket.SendAsync(json, success => { });
        }

        public void HandleJson(string json)
        {
            var reply = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            if (reply == null || !reply.ContainsKey("replyID")) return;
            var replyId = long.Parse(reply["replyID"].ToString());
            if (_callbacks.ContainsKey(replyId))
            {
                var callback = _callbacks[replyId];
                var resultJson = string.Empty;
                if (reply.ContainsKey("result"))
                {
                    resultJson = reply["result"]?.ToString();
                    if (!string.IsNullOrWhiteSpace(resultJson) && resultJson != "null")
                    {
                        System.Diagnostics.Debug.WriteLine("WS: {0}".ToFormat(json));
                    }
                }
                callback?.Invoke(resultJson);
            }
        }

        public void Dispose()
        {
            _webSocket.CloseAsync(CloseStatusCode.Normal);
        }
    }
}
