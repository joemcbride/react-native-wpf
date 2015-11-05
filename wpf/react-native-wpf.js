/**
 * @providesModule ReactNativeWPF
 * @flow
 */
'use strict';

var ReactChildren = require('ReactChildren');
var ReactClass = require('ReactClass');
var ReactComponent = require('ReactComponent');
let ReactElement = require('ReactElement');
var ReactElementValidator = require('ReactElementValidator');
var ReactNativeMount = require('ReactNativeMount');
var ReactPropTypes = require('ReactPropTypes');

var findNodeHandle = require('findNodeHandle');
var invariant = require('invariant');
var onlyChild = require('onlyChild');
var warning = require('warning');

var createElement = ReactElement.createElement;
var createFactory = ReactElement.createFactory;
var cloneElement = ReactElement.cloneElement;

var ReactNativeDefaultInjectionWPF = require('ReactNativeDefaultInjectionWPF');
ReactNativeDefaultInjectionWPF.inject();

if (__DEV__) {
  createElement = ReactElementValidator.createElement;
  createFactory = ReactElementValidator.createFactory;
  cloneElement = ReactElementValidator.cloneElement;
}

var render = function(
  element: ReactElement,
  mountInto: number,
  callback?: ?(() => void)
): ?ReactComponent {
  return ReactNativeMount.renderComponent(element, mountInto, callback);
};

let ReactNative = {
  hasReactNativeInitialized: false,
  Children: {
    map: ReactChildren.map,
    forEach: ReactChildren.forEach,
    count: ReactChildren.count,
    only: onlyChild
  },
  Component: ReactComponent,
  PropTypes: ReactPropTypes,
  createClass: ReactClass.createClass,
  createElement: createElement,
  createFactory: createFactory,
  cloneElement: cloneElement,
  findNodeHandle: findNodeHandle,
  render: render,
  unmountComponentAtNode: ReactNativeMount.unmountComponentAtNode,

 // Hook for JSX spread, don't use this for anything else.
  __spread: Object.assign,

  unmountComponentAtNodeAndRemoveContainer: ReactNativeMount.unmountComponentAtNodeAndRemoveContainer,

  isValidClass: ReactElement.isValidFactory,
  isValidElement: ReactElement.isValidElement,
};

module.exports = ReactNative;
