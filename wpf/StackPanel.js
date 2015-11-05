/**
 * @providesModule StackPanel
 */

var createReactNativeComponentClass =
  require('createReactNativeComponentClass');


var viewConfig = {
  validAttributes: {
    backgroundColor: true,
    orientation: true
  },
  uiViewClassName: 'ReactStackPanel',
};

var Container = createReactNativeComponentClass(viewConfig);

module.exports = Container;
