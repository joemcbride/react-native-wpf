/*
 * @providesModule ReactNativeDefaultInjectionWPF
 * @flow
 */
'use strict';

var EventPluginHub = require('EventPluginHub');
var EventPluginUtils = require('EventPluginUtils');
var IOSDefaultEventPluginOrder = require('IOSDefaultEventPluginOrder');
var IOSNativeBridgeEventPlugin = require('IOSNativeBridgeEventPlugin');
var ReactComponentEnvironment = require('ReactComponentEnvironment');
var ReactDefaultBatchingStrategy = require('ReactDefaultBatchingStrategy');
var ReactInstanceHandles = require('ReactInstanceHandles');
var ResponderEventPlugin = require('ResponderEventPlugin');
var ReactNativeComponentEnvironment = require('ReactNativeComponentEnvironment');
var ReactNativeGlobalInteractionHandler = require('ReactNativeGlobalInteractionHandler');
var ReactNativeGlobalResponderHandler = require('ReactNativeGlobalResponderHandler');
var ReactNativeMount = require('ReactNativeMount');
var ReactUpdates = require('ReactUpdates');
var ResponderEventPlugin = require('ResponderEventPlugin');

require('RCTEventEmitter');

function inject() {
  EventPluginHub.injection.injectEventPluginOrder(IOSDefaultEventPluginOrder);
  EventPluginHub.injection.injectInstanceHandle(ReactInstanceHandles);

  ResponderEventPlugin.injection.injectGlobalResponderHandler(
    ReactNativeGlobalResponderHandler
  );

  ResponderEventPlugin.injection.injectGlobalInteractionHandler(
    ReactNativeGlobalInteractionHandler
  );

  /**
   * Some important event plugins included by default (without having to require
   * them).
   */
  EventPluginHub.injection.injectEventPluginsByName({
    'ResponderEventPlugin': ResponderEventPlugin,
    'IOSNativeBridgeEventPlugin': IOSNativeBridgeEventPlugin
  });

  ReactUpdates.injection.injectReconcileTransaction(
    ReactNativeComponentEnvironment.ReactReconcileTransaction
  );

  ReactUpdates.injection.injectBatchingStrategy(
    ReactDefaultBatchingStrategy
  );

  ReactComponentEnvironment.injection.injectEnvironment(
    ReactNativeComponentEnvironment
  );

  EventPluginUtils.injection.injectMount(ReactNativeMount);
}

module.exports = {
  inject: inject
};
