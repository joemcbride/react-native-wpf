/**
 * @providesModule Container
 */

var createReactNativeComponentClass =
  require('createReactNativeComponentClass');


var viewConfig = {
  validAttributes: {
    backgroundColor: true
  },
  uiViewClassName: 'ReactContainer',
};

var Container = createReactNativeComponentClass(viewConfig);

module.exports = Container;
