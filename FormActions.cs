using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

public class FormActions
{
    private IWebDriver driver;

    public FormActions(IWebDriver driver)
    {
        this.driver = driver;
    }

    public void EnterText(IWebElement element, string value)
    {
        element.Clear();
        element.SendKeys(value);
    }

    public void SelectRadio(IWebElement element)
    {
        if (!element.Selected)
        {
            try
            {
                element.Click();
            }
            catch (ElementClickInterceptedException)
            {
                // Fallback to JavaScript click if UI overlay/label intercepts click
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", element);
            }
        }
    }

    public void SelectFirstValidOption(IWebElement element)
    {
        SelectElement select = new SelectElement(element);

        if (select.Options.Count <= 1)
        {
            throw new Exception("Dropdown does not contain a valid option.");
        }

        for (int i = 0; i < select.Options.Count; i++)
        {
            string text = select.Options[i].Text.Trim();

            if (text != "")
            {
                string lower = text.ToLower();
                if (!lower.Contains("select"))
                {
                    select.SelectByIndex(i);
                    Console.WriteLine("Selected State: " + text);
                    return;
                }
            }
        }

        throw new Exception("Could not find a valid State option.");
    }
}

