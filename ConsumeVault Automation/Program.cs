using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;


namespace MediumHTTPExample
{
    class Program
    {
        public static IWebDriver driver;
        static IJavaScriptExecutor js;
        // The since CS 7.1 TYPE can be Task instead of void
        static async Task Main(string[] args)
        {
            // Creating an instance of the HTTP Client
            HttpClient HttpClient = HttpClientFactory.Create();
            //Setting the URL to our HashiCorp Secret
            string url = "http://127.0.0.1:8200/v1/secret/data/credentials";
            // Setting the Token Header
            HttpClient.DefaultRequestHeaders.Add("X-Vault-Token", "root");
            // Setting the Content-type to application/json
            HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            // Making the HTTP Get call to consult our Secret
            JObject json= JObject.Parse(await HttpClient.GetStringAsync(url));
            // Printing the response
            Console.WriteLine(json);
            // Storing the key-value pairs of our secret from the response
            JToken secrets = json["data"]["data"];
            // Validating the previous statement is true
            Console.WriteLine("\n" + secrets);
            // Storing our key-value pairs to a Dictionary for future data manipulation
            Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(secrets.ToString());
            // Looping through our key-value pairs
            foreach (var item in values)
            {
                // Printing our key-value pairs
                Console.WriteLine($"Key: {item.Key} Value: {item.Value}");
            }
            var options = new ChromeOptions();
            //options.AddArguments("--start-maximized", @"C:\Users\Marco Urrea.MARCO-URREA\source\repos\ConsumeVault Automation\bin\Debug\netcoreapp2.2\chromedriver.exe");
            driver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            js = (IJavaScriptExecutor)driver;
            driver.Url = "https://dev53074.service-now.com/navpage.do";
            driver.SwitchTo().Frame(driver.FindElement(By.Name("gsft_main")));
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(
            By.XPath("//*[@id='user_name']")));
            js.ExecuteScript("document.getElementById('user_name').setAttribute('value',\'" + values["user"] + "\')");
            js.ExecuteScript("document.getElementById('user_password').setAttribute('value',\'" + values["password"] + "\')");
            driver.FindElement(By.XPath("//*[@id='sysverb_login']")).Click();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(270));
            Console.WriteLine("Hit Any key to exit...");
            Console.ReadKey();

            driver.Quit();
            driver.Quit();
        }
    }
}
