using OpenQA.Selenium;
using System;
using System.Collections.Generic;

public class ResilientElementFinder
{
    private IWebDriver driver;

    public ResilientElementFinder(IWebDriver driver)
    {
        this.driver = driver;
    }

    public IWebElement FindElement(FieldDescriptor field)
    {
        List<IWebElement> candidates = new List<IWebElement>();

        if (field.Type == "text")
        {
            candidates.AddRange(
                driver.FindElements(By.CssSelector(
                    "input:not([type='radio']):not([type='checkbox']):not([type='hidden'])")));
            candidates.AddRange(
                driver.FindElements(By.TagName("textarea")));
        }
        else if (field.Type == "radio")
        {
            candidates.AddRange(
                driver.FindElements(By.CssSelector(
                    "input[type='radio']")));
        }
        else if (field.Type == "dropdown")
        {
            candidates.AddRange(
                driver.FindElements(By.TagName("select")));
        }

        IWebElement? bestElement = null;
        int bestScore = -1;

        foreach (IWebElement element in candidates)
        {
            int score = CalculateScore(element, field);

            Console.WriteLine(
                "Candidate: " + GetDescription(element) + " | Score: " + score);

            if (score > bestScore)
            {
                bestScore = score;
                bestElement = element;
            }
        }

        if (bestElement == null || bestScore < 20)
        {
            throw new Exception(
                "Could not confidently locate field: " + field.Name);
        }

        Console.WriteLine(
            "Selected: " + field.Name + " | Score: " + bestScore);

        return bestElement;
    }

    private int CalculateScore(
        IWebElement element, FieldDescriptor field)
    {
        int score = 0;

        string id = SafeAttribute(element, "id");
        string name = SafeAttribute(element, "name");
        string placeholder = SafeAttribute(element, "placeholder");
        string aria = SafeAttribute(element, "aria-label");
        string type = SafeAttribute(element, "type");

        string ownInformation = (id + " " + name + " " + placeholder + " " + aria).ToLower();

        foreach (string keyword in field.Keywords)
        {
            string key = keyword.ToLower();
            if (ownInformation.Contains(key))
            {
                score += 30;
            }
        }

        string context = GetContextText(element);
        context = context.ToLower();

        foreach (string keyword in field.Keywords)
        {
            string key = keyword.ToLower();
            if (context.Contains(key))
            {
                score += 25;
            }
        }

        if (field.Type == "text")
        {
            if (type == "" || type == "text")
            {
                score += 20;
            }
        }

        if (field.Type == "radio")
        {
            if (type == "radio")
            {
                score += 20;
            }

            string value = SafeAttribute(element, "value");
            string radioContext = GetExtendedContextText(element);

            foreach (string keyword in field.Keywords)
            {
                string key = keyword.ToLower();

                // Exact value match priority (avoids "female" false-matching substring "male")
                if (value.ToLower() == key)
                {
                    score += 50;
                }
                else if (value.ToLower().Contains(key) && (key != "male" || !value.ToLower().Contains("female")))
                {
                    score += 40;
                }

                // Check immediate context (e.g. parent label "Male")
                string immediateContext = GetContextText(element).ToLower();
                if (immediateContext.Trim() == key)
                {
                    score += 50;
                }
                else if (radioContext.ToLower().Contains(key))
                {
                    score += 40;
                }
            }
        }

        if (field.Type == "dropdown")
        {
            if (element.TagName.ToLower() == "select")
            {
                score += 30;
            }
        }

        return score;
    }

    private string GetExtendedContextText(IWebElement element)
    {
        try
        {
            IWebElement parent = element.FindElement(By.XPath("./.."));
            string text = parent.Text;

            try
            {
                IWebElement grandParent = parent.FindElement(By.XPath("./.."));
                text += " " + grandParent.Text;
            }
            catch
            {
            }

            return text;
        }
        catch
        {
            return "";
        }
    }

    private string GetContextText(IWebElement element)
    {
        try
        {
            return element.FindElement(By.XPath("./..")).Text;
        }
        catch
        {
            return "";
        }
    }

    private string SafeAttribute(
        IWebElement element, string attribute)
    {
        string? value = element.GetAttribute(attribute);
        if (value == null)
        {
            return "";
        }
        return value;
    }

    private string GetDescription(IWebElement element)
    {
        return element.TagName + " | id=" + SafeAttribute(element, "id") + " | name=" + SafeAttribute(element, "name") + " | type=" + SafeAttribute(element, "type");
    }
}
