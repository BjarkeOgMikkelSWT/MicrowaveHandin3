using System;
using System.Security.Cryptography.X509Certificates;
using Microwave.Classes.Boundary;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace Microwave.Test.Integration
{
    [TestFixture]
    public class Step5Cook
    {
        private IOutput _output;
        private ITimer _timer;
        private IPowerTube _tube;
        private CookController _cook;
        private IDisplay _display;
        private Light _light;
        private Door _door;
        private Button _powerButton;
        private Button _timerButton;
        private Button _startCancelButton;
        private UserInterface sut;

        [SetUp]
        public void Setup()
        {
            _output = Substitute.For<IOutput>();

            _timer = Substitute.For<ITimer>();

            _tube = Substitute.For<IPowerTube>();

            _display = Substitute.For<IDisplay>();

            _cook = new CookController(_timer, _display, _tube);

            _light = new Light(_output);

            _door = new Door();

            _powerButton = new Button();
            _timerButton = new Button();
            _startCancelButton = new Button();

            sut = new UserInterface(
                _powerButton,
                _timerButton,
                _startCancelButton,
                _door,
                _display,
                _light,
                _cook);

            //Complete double association
            _cook.UI = sut;
        }

        [TestCase(1)]
        [TestCase(9)]
        [TestCase(4)]
        [TestCase(11)]
        public void TestStartCookingStartsTimer(int nbr_timer_presses)
        {
            _powerButton.Press();
            for (var i = 0; i < nbr_timer_presses; i++)
            {
                _timerButton.Press();
            }
            _startCancelButton.Press();
            _timer.Received(1).Start(60*nbr_timer_presses);
        }

        [TestCase(1)]
        [TestCase(9)]
        [TestCase(7)]
        [TestCase(3)]
        public void TestStartCookingTurnsOnPowerTube(int nbr_power_presses)
        {
            for (var i = 0; i < nbr_power_presses; i++)
            {
                _powerButton.Press();
            }

            _timerButton.Press();
            _startCancelButton.Press();
            _tube.Received(1).TurnOn(50*nbr_power_presses);
        }

        [Test]
        public void TestCookControllerTurnsOffTubeWhenTimerExpired()
        {
            _powerButton.Press();
            _timerButton.Press();
            _startCancelButton.Press(); 
            _timer.Expired += Raise.EventWith(this, EventArgs.Empty);
            _tube.Received(1).TurnOff();
        }

        
        [Test]
        public void TestCookControllerCallsCookingDoneWhenTimerExpired()
        {
            _powerButton.Press();
            _timerButton.Press();
            _startCancelButton.Press();
            _timer.Expired += Raise.EventWith(this, EventArgs.Empty);
            _display.Received(1).Clear();
            _output.Received(1).OutputLine("Light is turned off");
        }

        
        [Test]
        public void TestValuesAreResetWhenCookControllerTimerExpired()
        {
            _powerButton.Press();
            _powerButton.Press();
            _timerButton.Press();
            _timerButton.Press();
            _startCancelButton.Press();
            _timer.Expired += Raise.EventWith(this, EventArgs.Empty);

            _powerButton.Press();
            _timerButton.Press();
            _display.Received(2).ShowPower(50);
            _display.Received(2).ShowTime(1,0);
        }

        [Test]
        public void TestCookControllerCallsUpdatesTimeOnTimerTick()
        {
            _powerButton.Press();
            _timerButton.Press();
            _startCancelButton.Press();
            _timer.TimeRemaining.Returns(59);
            _timer.TimerTick += Raise.EventWith(this, EventArgs.Empty);
            var temp = _timer.Received(1).TimeRemaining;
            _display.Received(1).ShowTime(0, 59);
        }

        [Test]
        public void TestCookStopCallsTimerStopAndTurnOff()
        {
            _powerButton.Press();
            _timerButton.Press();
            _startCancelButton.Press();
            _startCancelButton.Press();
            _timer.Received(1).Stop();
            _tube.Received(1).TurnOff();
        }

        [TearDown]
        public void TearDown()
        {

        }
    }
}