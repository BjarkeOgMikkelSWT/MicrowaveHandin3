using System.Security.Cryptography.X509Certificates;
using Microwave.Classes.Boundary;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace Microwave.Test.Integration
{
    [TestFixture]
    public class Step1Button
    {
        private ICookController _cook;
        private IDisplay _display;
        private ILight _light;
        private IDoor _door;
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
            _door = Substitute.For<IDoor>();
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

        [TestCase(1)]
        [TestCase(4)]
        [TestCase(7)]
        [TestCase(14)]
        public void TestPowerButtonCausesCallToShowPower(int nbrPowerPresses)
        {
            for (var i = 0; i < nbrPowerPresses; i++)
            {
                _powerButton.Press();
                _display.Received(1).ShowPower(50 * i + 50);
            }
        }

        [Test]
        public void TestPowerButtonPressed15TimesCausesOverflow()
        {

            for (var i = 0; i < 15; i++)
            {
                _powerButton.Press();
            }

            _display.Received(2).ShowPower(50);
            for (var i = 1; i < 14; i++)
            {
                
                _display.Received(1).ShowPower(50 * i + 50);
            }
        }

        [Test]
        public void TestPowerButtonPressed28TimesCausesAllPowerSettingsTwice()
        {

            for (var i = 0; i < 28; i++)
            {
                _powerButton.Press();
            }
            for (var i = 0; i < 14; i++)
            {

                _display.Received(2).ShowPower(50 * i + 50);
            }
        }

        [Test]
        public void TestStartCancelPressedRightAfterPowerButtonResetsDisplay()
        {
            _powerButton.Press();
            _startCancelButton.Press();

            _display.Received(1).Clear();
        }

        [Test]
        public void TestStartCancelPressedRightAfterPowerButtonResetsValues()
        {
            _powerButton.Press();
            _powerButton.Press();
            _powerButton.Press();
            _startCancelButton.Press();
            _powerButton.Press();

            _display.Received(1).Clear();
            _display.Received(2).ShowPower(50);
            _display.Received(1).ShowPower(100);
            _display.Received(1).ShowPower(150);
        }


        [Test]
        public void TestSetTimePressedDoesNotCallDisplayTimeWhenTheStateIsReady()
        {
            _timerButton.Press();
            _display.DidNotReceiveWithAnyArgs().ShowTime(0,0);
        }

        [TestCase(1)]
        [TestCase(3)]
        [TestCase(10)]
        [TestCase(20)]
        public void TestSetTimePressedIncreasesTimeByOneMinuteWhenStateIsSet_Power(int nbr_times_pressed)
        {
            _powerButton.Press();
            for (var i = 0; i < nbr_times_pressed; i++)
            {
                _timerButton.Press();
                _display.Received(1).ShowTime(i + 1, 0);
            }
        }


        [Test]
        public void TestStartCancelStartsCookingAfterTimeIsSet()
        {
            _powerButton.Press();
            _timerButton.Press();
            _timerButton.Press();
            _startCancelButton.Press();

            _light.Received(1).TurnOn();
            _cook.Received(1).StartCooking(50, 120);
        }

        [Test]
        public void TestStartCancelStartsStopsCookingAfterStart()
        {
            _powerButton.Press();
            _timerButton.Press();
            _timerButton.Press();
            _startCancelButton.Press();
            _startCancelButton.Press();

            _cook.Received(1).Stop();
            _display.Received(1).Clear();
            _light.Received(1).TurnOff();
            
        }
    }
}