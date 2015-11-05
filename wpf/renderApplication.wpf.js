/**
 * @providesModule renderApplicationWPF
 */
let React = require('ReactNativeWPF');
let invariant = require('invariant');

function renderApplication(RootComponent, initialProps, rootTag) {

  //console.log('renderApplication', rootTag, initialProps);

  invariant(
    rootTag,
    'Expect to have a valid rootTag, instead got ', rootTag
  );

  React.render(
    <RootComponent
      {...initialProps}
      rootTag={rootTag}
    />,
    rootTag
  );
};

module.exports = renderApplication;
