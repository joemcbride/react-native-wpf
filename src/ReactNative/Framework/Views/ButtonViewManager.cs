using Caliburn.Micro;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;

namespace ReactNative.Framework.Views
{
    public class ReactButtonView : Button
    {
        public ReactButtonView()
        {
            SetBinding(ContentProperty, "Text");
            SetBinding(CommandProperty, "ClickCommand");
        }
    }

    public class ReactButtonViewModel : ReactComponentBase
    {
        private readonly IEventDispatcher _eventDispatcher;
        private ICommand _clickCommand;

        public ReactButtonViewModel(IEventDispatcher eventDispatcher)
        {
            _eventDispatcher = eventDispatcher;
        }

        public string Text { get; set; }

        public ICommand ClickCommand
        {
            get
            {
                if (_clickCommand == null)
                {
                    _clickCommand = new RelayCommand(param => Click(), param => true);
                }
                return _clickCommand;
            }
        }

        public void Click()
        {
            _eventDispatcher.SendInputEventWithName("click", new Dictionary<string, object>
            {
                { "target", ReactTag.Value }
            });
        }
    }

    [ReactModule("ReactButton")]
    public class ButtonViewManager : ViewManager
    {
        public override IReactComponent View()
        {
            return IoC.Get<ReactButtonViewModel>();
        }
    }
}