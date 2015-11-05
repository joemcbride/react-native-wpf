# React Native WPF

[2 minute React Native WPF Demo Video](https://www.youtube.com/watch?v=9n5aJfY0ewY)

![](http://i.imgur.com/ZO9MGLu.png)

## Noted Dependencies

* VS2015
* react-native 0.9.0
* react-native-cli 0.1.4
* JavaScriptCore (written in C) .NET wrapper (included)
* CefSharp (alternate for embedded runtime)
* Caliburn.Micro

## Getting Started

```
npm install
```

```
npm install -g react-native-cli@0.1.4
```

You will need to edit one of the core React Native files because a custom application renderer is required.  Find `AppRegistry.js` file (should be at `node_modules\react-native\Libraries\AppRegistry\AppRegistry.js`) and change `require('renderApplication')` to `require('renderApplicationWPF')`.

```
npm run wpf
```

This will run the React Native packager.  Browse to:

```
http://localhost:3333/debugger-ui
```

Build and run the WPF project.  This should run and connect to the packager server via websockets.  Note that if you do not have C++ installed or do not want to use the JavaScriptCore embedded runtime, you can remove those projects from the solution or set them to not build.

The sample application source files can be found in the root directory.

```
/wpf/index.ios.js
```

You can build the stand-alone bundle with the following command (you will need react-native-cli installed):

```
npm run bundle
```

## Help

* If you are trying to run the JavaScriptCore version, ensure the dlls found in `/JavaScriptCore/deps/` are in the bin directory.
* Sometimes the CefSharp files can get locked during the build process.  I've found just canceling the build and re-running it helps.

## Why

This is a prototype to see if react-native could drive WPF.  This is not production ready and a lot of this code is "spike code".  I am also not a C++ developer so please excuse any atrocities there.  The main purpose was to see if this could be integrated into an existing WPF codebase (for the purpose of eventually moving the entire app to the web).  If you are starting a new desktop project and want to use JavaScript, use something like github's [Electron](http://electron.atom.io/).

## Roadmap

As of right now I have no plans to further this project, though it would be interesting to see how React 0.14 could change things.  As always though, pull requests accepted.
