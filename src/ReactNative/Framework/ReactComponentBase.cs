using Caliburn.Micro;

namespace ReactNative.Framework
{
    public class ReactComponentBase : PropertyChangedBase, IReactComponent
    {
        private long? _reactTag;
        private string _backgroundColor;

        public long? ReactTag
        {
            get { return _reactTag; }
            set
            {
                _reactTag = value;
                NotifyOfPropertyChange(() => ReactTag);
            }
        }

        public string BackgroundColor
        {
            get { return _backgroundColor; }
            set
            {
                _backgroundColor = value;
                NotifyOfPropertyChange(() => BackgroundColor);
            }
        }
    }
}