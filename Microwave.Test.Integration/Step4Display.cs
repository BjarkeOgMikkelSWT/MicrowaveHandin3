using System.Security.Cryptography.X509Certificates;
using Microwave.Classes.Boundary;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace Microwave.Test.Integration
{
    [TestFixture]
    public class Step4Display
    {
        private IOutput _output;
        private ICookController _cook;
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
            _cook = Substitute.For<ICookController>();
            _display = new Display(_output);
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
        }

        [Test]
        public void TestDisplayClear()
        {
            _powerButton.Press();
            _startCancelButton.Press();
            _output.Received(1).OutputLine($"Display cleared");
        }

        [TestCase(1)]
        [TestCase(3)]
        [TestCase(5)]
        [TestCase(7)]
        public void TestDisplayPowerDisplaysCorrectPower(int nbr_power_presses)
        {
            for (var i = 0; i < nbr_power_presses; i++)
            {
                _powerButton.Press();
            }

            _output.Received(1).OutputLine($"Display shows: {nbr_power_presses * 50} W");
        }

        [TestCase(1)]
        [TestCase(3)]
        [TestCase(5)]
        [TestCase(7)]
        public void TestDisplayTimerDisplaysCorrectTime(int nbr_time_presses)
        {
            _powerButton.Press();
            for (var i = 0; i < nbr_time_presses; i++)
            {
                _timerButton.Press();
            }

            _output.Received(1).OutputLine($"Display shows: {nbr_time_presses:D2}:{0:D2}");
        }

        [TearDown]
        public void TearDown()
        {

        }
    }
}