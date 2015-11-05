using System.Net.Http;
using System.Threading.Tasks;
using Caliburn.Micro;
using ReactNative.Framework;
using ReactNative.Framework.Views;

namespace SampleApp
{
    public class WebSocketSampleViewModel : Screen
    {
        private const int Port = 3333;

        private readonly HttpClient _httpClient;
        private ReactRootViewModel _reactScreen;
        private bool _autoReload = true;

        public WebSocketSampleViewModel()
        {
            _httpClient = new HttpClient();

            ReactScreen = IoC.Get<ReactRootViewModel>();
            Reload();
        }

        public ReactRootViewModel ReactScreen
        {
            get { return _reactScreen; }
            set
            {
                _reactScreen = value;
                NotifyOfPropertyChange(() => ReactScreen);
            }
        }

        public bool AutoReload
        {
            get { return _autoReload; }
            set
            {
                _autoReload = value;
                NotifyOfPropertyChange(() => AutoReload);

                if (value)
                {
                    WatchForChanges();
                }
                else
                {
                    _httpClient.CancelPendingRequests();
                }
            }
        }

        public void Reload()
        {
            var sourceUrl = "http://localhost:{0}/wpf/index.ios.bundle".ToFormat(Port);
            ReactScreen.RunApplication("SampleApp", null, sourceUrl);

            if (AutoReload)
            {
                WatchForChanges();
            }
        }

        public async Task WatchForChanges()
        {
            var url = "http://localhost:{0}/onchange".ToFormat(Port);

            _httpClient.CancelPendingRequests();

            await _httpClient.GetAsync(url);

            if (AutoReload)
            {
                Reload();
            }
        }
    }
}
