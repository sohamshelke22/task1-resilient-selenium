using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.IO;
using System.Text.Json;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            string url = "http://app.cloudqa.io/home/AutomationPracticeForm";
            string firstName = "Soham";
            string gender = "Male";

            string configPath = "appsettings.json";
            if (!File.Exists(configPath))
            {
                configPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
            }

            if (File.Exists(configPath))
            {
                string json = File.ReadAllText(configPath);
                JsonDocument config = JsonDocument.Parse(json);

                if (config.RootElement.TryGetProperty("Url", out var urlProp))
                {
                    url = urlProp.GetString() ?? url;
                }
                if (config.RootElement.TryGetProperty("FirstName", out var fnProp))
                {
                    firstName = fnProp.GetString() ?? firstName;
                }
                if (config.RootElement.TryGetProperty("Gender", out var gProp))
                {
                    gender = gProp.GetString() ?? gender;
                }
            }

            if (args.Length > 0 && !string.IsNullOrWhiteSpace(args[0]))
            {
                url = args[0];
            }

            Console.WriteLine();
            Console.WriteLine("==============================");
            Console.WriteLine("RESILIENT SELENIUM TEST");
            Console.WriteLine("==============================");
            Console.WriteLine("URL: " + url);

            ChromeOptions options = new ChromeOptions();
            if ((args.Length > 1 && args[1] == "--headless") || Environment.GetEnvironmentVariable("HEADLESS") == "1")
            {
                options.AddArgument("--headless=new");
            }

            IWebDriver driver = new ChromeDriver(options);

            try
            {
                driver.Manage().Window.Maximize();
                driver.Navigate().GoToUrl(url);

                ResilientElementFinder finder = new ResilientElementFinder(driver);
                FormActions actions = new FormActions(driver);

                // --------------------------------
                // FIRST NAME
                // --------------------------------
                Console.WriteLine();
                Console.WriteLine("Testing First Name...");

                FieldDescriptor firstNameField = new FieldDescriptor(
                    "First Name",
                    "text",
                    new string[] { "first name", "firstname", "given name" }
                );

                IWebElement firstNameElement = finder.FindElement(firstNameField);
                actions.EnterText(firstNameElement, firstName);
                Console.WriteLine("First Name: PASS");

                // --------------------------------
                // GENDER
                // --------------------------------
                Console.WriteLine();
                Console.WriteLine("Testing Gender...");

                FieldDescriptor genderField = new FieldDescriptor(
                    "Gender",
                    "radio",
                    new string[] { "gender", gender.ToLower() }
                );

                IWebElement genderElement = finder.FindElement(genderField);
                actions.SelectRadio(genderElement);
                Console.WriteLine("Gender: PASS");

                // --------------------------------
                // STATE
                // --------------------------------
                Console.WriteLine();
                Console.WriteLine("Testing State...");

                FieldDescriptor stateField = new FieldDescriptor(
                    "State",
                    "dropdown",
                    new string[] { "state", "province", "region" }
                );

                IWebElement stateElement = finder.FindElement(stateField);
                actions.SelectFirstValidOption(stateElement);
                Console.WriteLine("State: PASS");

                // --------------------------------
                // FINAL RESULT
                // --------------------------------
                Console.WriteLine();
                Console.WriteLine("==============================");
                Console.WriteLine("ALL TESTS PASSED");
                Console.WriteLine("==============================");
            }
            finally
            {
                Console.WriteLine();
                Console.WriteLine("Press ENTER to close browser.");
                if (!Console.IsInputRedirected)
                {
                    Console.ReadLine();
                }
                driver.Quit();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine("==============================");
            Console.WriteLine("TEST FAILED");
            Console.WriteLine("==============================");
            Console.WriteLine("Reason: " + ex.Message);
            Console.WriteLine();
            Console.WriteLine("Press ENTER to exit.");
            if (!Console.IsInputRedirected)
            {
                Console.ReadLine();
            }
        }
    }
}
