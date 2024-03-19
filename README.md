# CaculationEngine

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat)&nbsp;
![.NET](https://img.shields.io/badge/.NET-Framework%204.8-512BD4?style=flat)&nbsp;
![.NET](https://img.shields.io/badge/.NET-CoreApp%203.1-512BD4?style=flat)&nbsp;
<br>
[![CodeFactor](https://www.codefactor.io/repository/github/soomin-kevin-sung/dotnet-calculation-engine/badge)](https://www.codefactor.io/repository/github/soomin-kevin-sung/dotnet-calculation-engine)&nbsp;
[![License](https://img.shields.io/github/license/soomin-kevin-sung/dotnet-calculation-engine)](LICENSE.md)&nbsp;
![C#](https://img.shields.io/badge/.NET-C%23-007396?style=flat)&nbsp;

CalculationEngine is utlitiy for caculating formulas. It's inspired by [Jace.NET](https://github.com/pieterderycke/Jace), and some code was taken from [Jace.NET](https://github.com/pieterderycke/Jace).

## Features
* The CalculationEngine can calculate object
    - Calculating String Constant And Variable
        ```csharp
        var engine = new CalculationEngine();
        var variables = new Dictionary<string, object>() { { "code", "PRESENT" } };

        var answer = engine.Calculate("if(CODE == \"PRESENT\", \"TRUE\", \"FALSE\")", variables);

        ////////////// RESULT
        // answer : "TRUE"
        ```
* The CalculationEngine can access the object properties by [PropertyConnector](https://github.com/soomin-kevin-sung/dotnet-calculation-engine/blob/master/src/KevinComponent/KevinComponent/Execution/PropertyConnector.cs)
    * Custom Class
        ```csharp
        public class TestPoint
        {
            public TestPoint(string name,  double x, double y)
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
    * Implement PropertyConnector
        ```csharp
        public class TestPropertyConnector : PropertyConnector
        {
            public TestPropertyConnector(bool caseSensitive) : base(caseSensitive)
            {
            }

            protected override object? OnGetPropertyValue(object target, string propertyName)
            {
                if (target is TestPoint testPoint)
                {
                    if (propertyName.Equals(ConvertPropertyName(".Name")))
                        return testPoint.Name;
                    else if (propertyName.Equals(ConvertPropertyName(".X")))
                        return testPoint.X;
                    else if (propertyName.Equals(ConvertPropertyName(".Y")))
                        return testPoint.Y;
                }

                return null;
            }
        }
        ```
    * Calculate with Custom Class and Properties.
        ```csharp
        var engine = new CalculationEngine(new CalculationEngineOptions()
        {
            PropertyConnectorFactory = caseSensitive => new TestPropertyConnector(caseSensitive)
        });
        var variables = new Dictionary<string, object>()
        {
            { "p1", new TestPoint("A", 0, 1) },
            { "p2", new TestPoint("B", 1.25, -2.56) },
            { "Code", "A" }
        };

        var answer = engine.Calculate("p1.x + p2.y", variables);
        var answer2 = engine.Calculate("if ( Code == p1.Name, \"True\", \"False\")", variables);

        ////////////// RESULT
        // answer : -2.56
        // answer2 : "True"
        ```