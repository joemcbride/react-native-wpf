namespace ReactNative.Framework.Views
{
    public class ReactTextBlockViewModel : ReactComponentBase
    {
        private string _text;

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                NotifyOfPropertyChange(() => Text);
            }
        }
    }

    [ReactModule("ReactTextBlock")]
    public class TextBlockViewManager : ViewManager
    {
        public override IReactComponent View()
        {
            return new ReactTextBlockViewModel();
        }
    }
}