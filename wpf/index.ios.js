'use strict';

const React = require('ReactNativeWPF');
const AppRegistry = require('AppRegistry');
const TextBlock = require('TextBlock');
const Button = require('Button');
const Container = require('Container'); // ItemsControl
const StackPanel = require('StackPanel');

class SampleApp extends React.Component {

  constructor(props) {
    super(props);

    this.state = {
      text: 'Hello WPF from React Native',
      backgroundColor: '#efefef',
      likes: 0,
      toggle: false,
      orientation: 'horizontal'
    };

    this.like = this.like.bind(this);
    this.reset = this.reset.bind(this);
    this.toggle = this.toggle.bind(this);
  }

  like() {
    console.log('liked');
    let likes = this.state.likes +1;
    this.setState({
      text: 'Hello WPF from React Native +' + likes,
      backgroundColor: '#33ff33',
      likes
    });
  }

  toggle() {
    console.log('toggle');
    let { toggle } = this.state;
    let newToggle = !toggle;
    let or = newToggle ? 'vertical' : 'horizontal';
    this.setState({
      toggle: newToggle,
      orientation: or
    });
  }

  reset() {
    this.setState({
      text: 'Hello WPF from React Native',
      backgroundColor: '#efefef',
      likes: 0,
      toggle: false,
      orientation: 'horizontal'
    });
  }

  render() {
    let displayText = null;
    if(this.state.toggle) {
      displayText = <TextBlock text="Hello!"/>;
    }
    return (
      <Container>
        <TextBlock
          text={this.state.text}
          backgroundColor={this.state.backgroundColor}/>
        {displayText}
        <StackPanel orientation={this.state.orientation}>
          <Button
            text={'Like'}
            onClick={this.like}/>
          <Button
            text={'Reset'}
            onClick={this.reset}/>
          <Button
            text={'Toggle'}
            onClick={this.toggle}/>
        </StackPanel>
      </Container>
    );
  }
}

AppRegistry.registerComponent('SampleApp', ()=> SampleApp);
