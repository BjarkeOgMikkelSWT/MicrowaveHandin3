using System;
using System.IO;
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
    public class Step9Output
    {
        private StringWriter str;
        private Output _output;
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
            _output = new Output();  

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

            str = new StringWriter();
            Console.SetOut(str);
        }


        //Light tests
        [Test]
        public void TestTurnOnLightOutputsToConsole()
        {
            _door.Open();
            Assert.That(str.ToString().Contains("Light is turned on"));
        }

        [Test]
        public void TestTurnOffLightOutputsToConsole()
        {
            _door.Open();
            _door.Close();
            Assert.That(str.ToString().Contains("Light is turned off"));
        }

        //Display tests
        [Test]
        public void TestShowPowerOutputsToConsole()
        {
            _powerButton.Press();
            Assert.That(str.ToString().Contains($"Display shows: {50} W"));
        }

        [Test]
        public void TestShowTimeOutputsCorrectlyToConsole()
        {
            _powerButton.Press();
            _timerButton.Press();
            Assert.That(str.ToString().Contains($"Display shows: {1:D2}:{0:D2}"));
        }

        [Test]
        public void TestDisplayClearClearsDisplay()
        {
            _powerButton.Press();
            _timerButton.Press();
            _startCancelButton.Press();
            _startCancelButton.Press();
            Assert.That(str.ToString().Contains($"Display cleared"));
        }


        //Power tube tests
        [Test]
        public void TestPowerTubeOnOutputsCorrectly()
        {
            _powerButton.Press();
            _timerButton.Press();
            _startCancelButton.Press();
            Assert.That(str.ToString().Contains($"PowerTube works with {50}"));
        }

        [Test]
        public void TestPowerTubeOffOutputsCorrectly()
        {
            _powerButton.Press();
            _timerButton.Press();
            _startCancelButton.Press();
            _startCancelButton.Press();
            Assert.That(str.ToString().Contains($"PowerTube turned off"));
        }



        [TearDown]
        public void TearDown()
        {

        }
    }
}