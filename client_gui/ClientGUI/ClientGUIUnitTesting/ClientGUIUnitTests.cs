using ClientGUI.Controllers;

namespace ClientGUIUnitTesting
{
    public class ClientGUIUnitTests
    {
        private HomeController sut; //sut = system under test

        [SetUp]
        public void Setup()
        {
            sut = new HomeController();
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}