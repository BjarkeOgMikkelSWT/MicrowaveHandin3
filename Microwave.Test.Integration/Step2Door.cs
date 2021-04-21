using System.Security.Cryptography.X509Certificates;
using Microwave.Classes.Boundary;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace Microwave.Test.Integration
{
    [TestFixture]
    public class Step2Door
    {
        private ICookController _cook;
        private IDisplay _display;
        private ILight _light;
        private Door _door;
        private Button _powerButton;
        private Button _timerButton;
        private Button _startCancelButton;
        private UserInterface sut;

        [SetUp]
        public void Setup()
        {
            _cook = Substitute.For<ICookController>();
            _display = Substitute.For<IDisplay>();
            _light = Substitute.For<ILight>();
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
        public void TestDoorOpenWhenReadyTurnsOnLight()
        {
            _door.Open();
            _light.Received(1).TurnOn();
        }

        [Test]
        public void TestDoorClosedWhenDoorIsOpenTurnsOffLight()
        {
            _door.Open();
            _door.Close();
            _light.Received(1).TurnOff();
        }

        [Test]

        public void TestDoorOpenedWhenSetPowerClearsDisplay()
        {
            _powerButton.Press();
            _door.Open();
            _display.Received(1).Clear();
            
        }

        [Test]

        public void TestDoorOpenedWhenSetPowerTurnsOnLight()
        {
            _powerButton.Press();
            _door.Open();
            _light.Received(1).TurnOn();
        }

        [Test]
        public void TestDoorOpenedWhenSetPowerResetsValues()
        {
            _powerButton.Press();
            _powerButton.Press();
            _door.Open();
            _door.Close();
            _powerButton.Press();
            _display.Received(2).ShowPower(50);
        }

        public void TestDoorOpenedWhenSetTimeClearsDisplay()
        {
            _powerButton.Press();
            _timerButton.Press();
            _door.Open();
            _display.Received(1).Clear();

        }

        [Test]

        public void TestDoorOpenedWhenSetTimeTurnsOnLight()
        {
            _powerButton.Press();
            _timerButton.Press();
            _door.Open();
            _light.Received(1).TurnOn();
        }

        [Test]
        public void TestDoorOpenedWhenSetTimeResetsValues()
        {
            _powerButton.Press();
            _timerButton.Press();
            _timerButton.Press();
            _door.Open();
            _door.Close();
            _powerButton.Press();
            _timerButton.Press();
            _display.Received(2).ShowTime(1, 0);
        }

        public void TestDoorOpenedWhenCookingClearsDisplay()
        {
            _powerButton.Press();
            _timerButton.Press();
            _startCancelButton.Press();
            _door.Open();
            _display.Received(1).Clear();

        }

        [Test]

        public void TestDoorOpenedWhenCookingStopsCooking()
        {
            _powerButton.Press();
            _timerButton.Press();
            _startCancelButton.Press();
            _door.Open();
            _cook.Received(1).Stop();
        }

        [Test]
        public void TestDoorOpenedWhenCookingResetsValues()
        {
            _powerButton.Press();
            _powerButton.Press();
            _timerButton.Press();
            _timerButton.Press();
            _startCancelButton.Press();
            _door.Open();
            _door.Close();
            _powerButton.Press();
            _timerButton.Press();
            _display.Received(2).ShowPower(50);
            _display.Received(2).ShowTime(1, 0);
        }



        [TearDown]
        public void TearDown()
        {

        }
    }
}