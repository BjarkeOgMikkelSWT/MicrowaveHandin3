using System.Security.Cryptography.X509Certificates;
using Microwave.Classes.Boundary;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace Microwave.Test.Integration
{
    [TestFixture]
    public class Step3Light
    {
        private IOutput _output;
        private ICookController _cook;
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
            _cook = Substitute.For<ICookController>();
            _display = Substitute.For<IDisplay>();
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
        public void TurnOnLightWritesLightOnToOutput()
        {
            _door.Open();
            _output.Received(1).OutputLine("Light is turned on");
        }

        [Test]
        public void TurnOffLightWritesLightOffToOutput()
        {
            _door.Open();
            _door.Close();
            _output.Received(1).OutputLine("Light is turned off");
        }

        [TearDown]
        public void TearDown()
        {

        }
    }
}