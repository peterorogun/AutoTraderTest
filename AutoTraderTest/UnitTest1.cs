using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading;
using System.Linq;

namespace AutoTraderTest
{
    public class Tests
    {
        public IWebDriver driver;
        [SetUp]
        public void Setup()
        {
            ChromeOptions option = new ChromeOptions();
            option.AddArguments("--start-maximized", "--incognito");
            driver = new ChromeDriver(option);
            driver.Navigate().GoToUrl("https://www.autotrader.co.uk/");
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(30);
        }

        [Test]
        public void Test1()
        {
            var postCode = 
                driver.FindElement(
                    By.XPath("//input[@name='postcode']"));
            try
            {
                if (postCode.Displayed)
                {
                    //switch to iframe
                    Thread.Sleep(2000);
                    driver.SwitchTo().Frame(
                        driver.FindElement(By.XPath("//*[contains(@id, 'sp_message_iframe')]")));
                    driver.FindElement(By.XPath("//*[contains(@title, 'Accept')]")).Click();
                    driver.SwitchTo().DefaultContent(); //Switch back to default content

                    //Search config
                    postCode.SendKeys("WV11 1XE");
                    var distance = driver.FindElement(By.Name("distance"));
                    var SelectDistance = new SelectElement(distance);
                    SelectDistance.SelectByValue("15");

                    Thread.Sleep(1000);
                    var make = driver.FindElement(By.Id("make"));
                    var SelectMake = new SelectElement(make);
                    SelectMake.SelectByValue("Mercedes-Benz");

                    Thread.Sleep(1000);
                    var model = driver.FindElement(By.Id("model"));
                    var SelectModle = new SelectElement(model);
                    SelectModle.SelectByValue("C Class");

                    var minPrice = driver.FindElement(By.Id("minPrice"));
                    var SelectMinPrice = new SelectElement(minPrice);
                    SelectMinPrice.SelectByValue("100000");

                    var maxPrice = driver.FindElement(By.Id("maxPrice"));
                    var SelectMaxPrice = new SelectElement(maxPrice);
                    SelectMaxPrice.SelectByValue("250000");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            //Search filter
            var search = driver.FindElement(By.XPath("//*[@type='submit']"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", search);

            
            //Search result
            var searchResult = 
                driver.FindElements(By.XPath("//a[@data-label='search appearance click ']"));

                var itemToclick = searchResult[0].Text;
                new Actions(driver).MoveToElement(searchResult[0]).Perform();
                ((IJavaScriptExecutor)driver)
                    .ExecuteScript("arguments[0].style.border = '3px dotted blue'", searchResult[0]);
                searchResult[0].Click();

            //var ClickedItem = driver.FindElement(By.XPath("//h1[@data-testid='advert-title']"));
            //Validate that Selected vehicle contains filtered vehicle
            Thread.Sleep(3000);
            Assert.IsTrue(driver.FindElement(
                By.XPath("//h1[@data-testid='advert-title']"))
                .Text.Contains("Mercedes-Benz C Class"));
        }

        [TearDown]
        public void Quitbrowser()
        {
            driver.Quit();
        }
    }
}