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
    public class Step7PowerTube
    {
        private IOutput _output;
        private ITimer _timer;
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

            _timer = Substitute.For<ITimer>();

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
        [TestCase(3)]
        [TestCase(11)]
        [TestCase(9)]
        public void TestPowerTubeTurnOnOutputsCorrectString(int nbr_power_presses)
        {
            for (var i = 0; i < nbr_power_presses; i++)
            {
                _powerButton.Press();
            }

            _timerButton.Press();
            _startCancelButton.Press();
            _output.Received(1).OutputLine($"PowerTube works with {50*nbr_power_presses}");
        }

        [TearDown]
        public void TearDown()
        {

        }
    }
}