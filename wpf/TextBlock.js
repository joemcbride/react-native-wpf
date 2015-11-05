/**
 * @providesModule TextBlock
 */

var createReactNativeComponentClass =
  require('createReactNativeComponentClass');


var viewConfig = {
  validAttributes: {
    text: true,
    backgroundColor: true
  },
  uiViewClassName: 'ReactTextBlock',
};

var TextBlock = createReactNativeComponentClass(viewConfig);

module.exports = TextBlock;
