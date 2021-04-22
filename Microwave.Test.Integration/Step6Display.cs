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
    public class Step6Display
    {
        private IOutput _output;
        private ITimer _timer;
        private IPowerTube _tube;
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

            _timer = Substitute.For<ITimer>();

            _tube = Substitute.For<IPowerTube>();

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


        [Test]
        public void TestDisplayOutputsCorrectStringWhenCalledByCookController()
        {
            _powerButton.Press();
            _timerButton.Press();
            _startCancelButton.Press();
            _timer.TimeRemaining.Returns(59);
            _timer.TimerTick += Raise.EventWith(this, EventArgs.Empty);
            var temp = _timer.Received(1).TimeRemaining;
            _output.Received(1).OutputLine($"Display shows: {0:D2}:{59:D2}");
        }

        [TearDown]
        public void TearDown()
        {

        }
    }
}