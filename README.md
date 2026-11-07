# Resilient Selenium Automation - C#

## Overview
This project automates three fields from the CloudQA Automation Practice Form:
1. First Name - text input
2. Gender - radio button
3. State - dropdown

The main objective is to make the automation resilient to common UI changes instead of relying only on fixed IDs, names, classes, or one XPath.

## Technology
- C#
- .NET
- Selenium WebDriver
- Google Chrome

## Website
http://app.cloudqa.io/home/AutomationPracticeForm

## Features
- Configurable target URL
- Semantic field identification
- Candidate scoring
- Context-aware element detection
- First Name automation
- Gender radio-button automation
- State dropdown automation
- PASS/FAIL reporting

## Project Structure
```text
Task1-ResilientSelenium/
│
├── Program.cs
├── FieldDescriptor.cs
├── ResilientElementFinder.cs
├── FormActions.cs
├── appsettings.json
└── README.md
```

## Setup
Install .NET SDK (8.0+ / 10.0+).
Then run:
```bash
dotnet restore
```

## Execute
```bash
dotnet run
```
Or provide another URL:
```bash
dotnet run -- "http://app.cloudqa.io/home/AutomationPracticeForm"
```

## Approach
The automation does not depend exclusively on fixed IDs or CSS classes. For each required field, the program creates multiple candidate elements and assigns scores based on:
- Element type
- ID
- Name
- Placeholder
- ARIA label
- Nearby text
- Parent context
- Field-specific keywords

The candidate with the highest confidence score is selected.

## Why this approach?
A fixed locator such as:
```csharp
By.Id("firstName")
```
can fail if the application changes the ID. The scoring approach allows the automation to continue working when individual HTML attributes or field ordering change, provided enough semantic information remains available.

## Limitations
This solution cannot guarantee resilience against arbitrary changes. If a field is renamed completely and has no meaningful label, accessibility information, placeholder, or surrounding context, the confidence score may become too low. Major structural changes, CAPTCHA, authentication requirements, and heavily dynamic applications may require additional handling. The current implementation also focuses only on the three fields required by the assignment rather than the iframe and Shadow DOM examples present on the practice page.

