# CaculationEngine

[![Build And Publish to Nuget](https://github.com/soomin-kevin-sung/dotnet-calculation-engine/actions/workflows/build_test_publish.yml/badge.svg?branch=master)](https://github.com/soomin-kevin-sung/dotnet-calculation-engine/actions/workflows/build_test_publish.yml)
![Nuget](https://img.shields.io/nuget/v/kevincomponent.calculationengine?label=NuGet&logo=NuGet)
<br>
![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat)&nbsp;
![.NET](https://img.shields.io/badge/.NET-Framework%204.8-512BD4?style=flat)&nbsp;
![.NET](https://img.shields.io/badge/.NET-CoreApp%203.1-512BD4?style=flat)&nbsp;
<br>
[![CodeFactor](https://www.codefactor.io/repository/github/soomin-kevin-sung/dotnet-calculation-engine/badge)](https://www.codefactor.io/repository/github/soomin-kevin-sung/dotnet-calculation-engine)&nbsp;
[![License](https://img.shields.io/github/license/soomin-kevin-sung/dotnet-calculation-engine)](LICENSE.md)&nbsp;
![C#](https://img.shields.io/badge/.NET-C%23-007396?style=flat)&nbsp;

**CalculationEngine** is a useful tool for calculating complex formulas.
<br>
It's inspired by [Jace.NET](https://github.com/pieterderycke/Jace), and some code was taken from [Jace.NET](https://github.com/pieterderycke/Jace).

## Simple Code
*  Here is a simple piece of code that demonstrates what a CalculationEngine is.
    ```csharp
    // Define CalculationEngine
    var engine = new CalculationEngine();

    // Calculate Formulas
    var answer = engine.Calculate("1 + 3");
    var answer2 = engine.Calculate("3 + 5 * 9");
    var answer3 = engine.Calculate("pow(3, 2) + (2 - 3)");

    /****************** RESULT ******************/
    // answer : 4
    // answer2 : 48
    // answer3 : 8
    ```

## Features
### The CalculationEngine can calculate formulas that contain strings and objects.
* Calculating String Constants And Variables
    ```csharp
    var engine = new CalculationEngine();
    var variables = new Dictionary<string, object>() { { "code", "PRESENT" } };

    var answer = engine.Calculate("if(CODE == \"PRESENT\", \"TRUE\", \"FALSE\")", variables);

    /****************** RESULT ******************/
    // answer : "TRUE"
    ```
<br>

### The CalculationEngine can access the object properties by [PropertyConnector](https://github.com/soomin-kevin-sung/dotnet-calculation-engine/blob/master/src/KevinComponent/KevinComponent/Execution/PropertyConnector.cs)
* Point Class for formula
    ```csharp
    public class Point
    {
        public Point(string name,  double x, double y)
        {
            Name = name;
            X = x;
            Y = y;
        }

        public string Name { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
    }
    ```
* Implementing a PropertyConnector to access Point class properties
    ```csharp
    public class PointPropertyConnector : PropertyConnector
    {
        public PointPropertyConnector(bool caseSensitive) : base(caseSensitive)
        {
        }

        protected override object? OnGetPropertyValue(object target, string propertyName)
        {
            if (target is Point point)
            {
                if (propertyName.Equals(ConvertPropertyName(".Name")))
                    return point.Name;
                else if (propertyName.Equals(ConvertPropertyName(".X")))
                    return point.X;
                else if (propertyName.Equals(ConvertPropertyName(".Y")))
                    return point.Y;
            }

            return null;
        }
    }
    ```
* Calculate formulas with the Point class
    ```csharp
    var engine = new CalculationEngine(new CalculationEngineOptions()
    {
        PropertyConnectorFactory = caseSensitive => new PointPropertyConnector(caseSensitive)
    });
    var variables = new Dictionary<string, object>()
    {
        { "p1", new Point("A", 0, 1) },
        { "p2", new Point("B", 1.25, -2.56) },
        { "Code", "A" }
    };

    var answer = engine.Calculate("p1.x + p2.y", variables);
    var answer2 = engine.Calculate("if ( Code == p1.Name, \"True\", \"False\")", variables);

    /****************** RESULT ******************/
    // answer : -2.56
    // answer2 : "True"
    ```