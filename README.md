# .NET Reflection and Expression Evaluation

This project demonstrates a string expression and will replace object values into the expression before evaluating. 

## Example

```csharp
var objA = new SampleModel { Prop1 = "hi2", Prop2 = 3 };
var objB = new SampleModel { Prop1 = "hi2", Prop2 = 4 };

var expression = "a.Prop1 == \"hi2\" && b.Prop2 >= 3";

var result = await _demoService.EvaluateExpression<bool>(
    expression,
    (objA, "a"),
    (objB, "b"));

// true
```

The reflection/script evaluation is in [DemoService](./ConsoleApp/Services/DemoService.cs#L28).
