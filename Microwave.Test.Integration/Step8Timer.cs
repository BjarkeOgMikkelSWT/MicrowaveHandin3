using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Microwave.Classes.Boundary;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;
using NSubstitute;
using NUnit.Framework;
using Timer = Microwave.Classes.Boundary.Timer;

namespace Microwave.Test.Integration
{
    [TestFixture]
    public class Step8Timer
    {
        private IOutput _output;
        private Timer _timer;
        private PowerTube _tube;
        private CookController _cook;
        private Display _display;
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

            _timer = new Timer();

            _tube = new PowerTube(_output);

            _display = new Display(_output);

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
        [TestCase(7)]
        [TestCase(4)]
        [TestCase(11)]
        public void TestTimerStartSetsTimeRemainingToExpectedValue(int nbr_timer_presses)
        {
            _powerButton.Press();
            for (var i = 0; i < nbr_timer_presses; i++)
            {
                _timerButton.Press();
            }

            _startCancelButton.Press();

            Assert.AreEqual(60*nbr_timer_presses, _timer.TimeRemaining);
        }

        [Test]
        public void TestTimerUpdateTimeAsExpected()
        {
            _powerButton.Press();
            _timerButton.Press();
            _startCancelButton.Press();
            Thread.Sleep(2100);
            _output.Received(1).OutputLine($"Display shows: {0:D2}:{59:D2}");
            _output.Received(1).OutputLine($"Display shows: {0:D2}:{58:D2}");
        }

        [Test]
        public void TestTimerExpiredIsCalledAtCorrectTime()
        {
            _powerButton.Press();
            _timerButton.Press();
            _startCancelButton.Press();
            Thread.Sleep(60100);
            _output.Received(1).OutputLine("PowerTube turned off");
            _output.Received(1).OutputLine("Display cleared");
            _output.Received(1).OutputLine("Light is turned off");

        }

        [Test]
        public void TestTimerStopStopsTimer()
        {
            _powerButton.Press();
            _timerButton.Press();
            _startCancelButton.Press();
            Thread.Sleep(1100);
            _startCancelButton.Press();
            Thread.Sleep(1100);
            Assert.AreEqual(59, _timer.TimeRemaining);
        }

        [TearDown]
        public void TearDown()
        {

        }
    }
}