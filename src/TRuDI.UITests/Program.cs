namespace TRuDI.UITests
{
    using System;
    using System.Linq;

    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using OpenQA.Selenium.Support.UI;

    class Program
    {
        static void Main(string[] args)
        {
            using (IWebDriver driver = new ChromeDriver(@".\bin\Debug\netcoreapp2.0"))
            {
                driver.Navigate().GoToUrl("http://localhost:5000");

                if (!driver.Title.StartsWith("TRuDI")) throw new Exception("TRuDI Backend not running.");

                driver.Url = "http://localhost:5000/About";
                driver.Navigate().Back();

                driver.Url = "http://localhost:5000/Help";
                driver.Navigate().Back();

                driver.FindElement(By.LinkText("Weiter")).Click();


                // Find the text input elements Username & Password
                IWebElement user = driver.FindElement(By.Name("Username"));
                IWebElement pass = driver.FindElement(By.Name("Password"));
                user.Clear();
                user.SendKeys("consumer");
                pass.Clear();
                pass.SendKeys("consumer");

                //fill in Device data
                IWebElement device = driver.FindElement(By.Id("DeviceId"));
                device.Clear();
                device.SendKeys("ELGZ0012345678");
                IWebElement address = driver.FindElement(By.Id("Address"));
                address.Clear();
                address.SendKeys("192.168.188.30");
                IWebElement port = driver.FindElement(By.Id("Port"));
                port.Clear();
                port.SendKeys("443");

                // Verbinden
                driver.FindElement(By.Id("connectButton")).Click();

                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                wait.Until(d => d.Title.StartsWith("TRuDI - Verträge", StringComparison.OrdinalIgnoreCase));

                if (!driver.Title.Contains("TRuDI - Verträge")) throw new Exception("Verbindung fehlgeschlagen.");
                Console.WriteLine("Page title is: " + driver.Title);


                // Tageswerte auslesen
                IWebElement tagesElement = driver.FindElement(By.Id("0_9_TAF6")).FindElement(By.TagName("input"));
                tagesElement.Click();
                IWebElement readBtn = driver.FindElements(By.Id("btnRead")).FirstOrDefault();
                if (readBtn != null) readBtn.Click();

                wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                wait.Until(d => d.Title.StartsWith("TRuDI - Abrechnungsdaten", StringComparison.OrdinalIgnoreCase));

                if (!driver.Title.Contains("TRuDI - Abrechnungsdaten")) throw new Exception("Auslesung fehlgeschlagen.");
                Console.WriteLine("Page title is: " + driver.Title);

                IWebElement omlDropdown = driver.FindElement(By.Id("tab-dropdown-ovl"));
                omlDropdown.Click();
                IWebElement oml = driver.FindElement(By.Id("#tab_ovl_1ISK0000000001_0100020800FF_900-tab"));
                oml.Click();

                Console.ReadLine();
            }
        }
    }
}
