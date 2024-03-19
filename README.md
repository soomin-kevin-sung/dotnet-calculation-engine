# CaculationEngine

CalculationEngine is utlitiy for caculating formulas. It's inspired by [Jace.NET](https://github.com/pieterderycke/Jace), and some code was taken from [Jace.NET](https://github.com/pieterderycke/Jace).

## Difference with [Jace.NET](https://github.com/pieterderycke/Jace)
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
    * Implement Custom PropertyConnector
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
    * Calculate with Object Property
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