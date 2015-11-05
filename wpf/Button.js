/**
 * @providesModule Button
 */

var createReactNativeComponentClass =
  require('createReactNativeComponentClass');


var viewConfig = {
  validAttributes: {
    text: true,
    backgroundColor: true
  },
  uiViewClassName: 'ReactButton',
};

var Button = createReactNativeComponentClass(viewConfig);

module.exports = Button;
