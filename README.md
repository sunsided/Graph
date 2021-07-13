# Graph / Dataflow

**Graph** is a .NET library for parallel filter-based pipeline/dataflow processing.

---

🧀 Please note that like any good cheese, this library too is old and smells a bit.
It was written in the years 2011 and 2012 and while still functional it is by no
means actively developed or maintained, nor do I intend to change that.
The code was updated to target .NET 6 and use more modern C# language constructs,
but that's about it. — That said, feel free to play around!

💡 For any serious use, please have a look at the [System.Threading.Tasks.Dataflow] package first,
as it provides the [Dataflow (Task Parallel Library)] functionality.
Unlike this library, TPL Dataflow supports `async` actions.

[System.Threading.Tasks.Dataflow]: https://www.nuget.org/packages/System.Threading.Tasks.Dataflow/
[Dataflow (Task Parallel Library)]: https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/dataflow-task-parallel-library

---

## Usage example

1. Create a data source

    ```C#
    ISource source = new LogicEmitter();
    ```

2. Create a processor

    ```C#
    IFilter filter = new NotGate();
    ```

3. Create a data sink

    ```C#
    ISink sink = new ActionInvoker<bool>(value => Trace.WriteLine("The value is: " + value));
    ```

4. Build the processing graph

    ```C#
    source.AttachOutput(filter);
    filter.AttachOutput(sink);
    ```

5. Start all sources

    ```C#
    source.StartProcessing();
    ```

    (Note: For the ```LogicEmitter``` to actually do something you would have to call one of its ```EmitXXX()``` methods. This is just an example though.)

6. ???
7. Profit

## Example filters

### Data processor with a single input

```C#
/// <summary>
/// Produces a logical NOT (inversion) of the input
/// </summary>
public sealed class NotGate : DataFilter<bool, bool>
{
    protected override bool ProcessData(bool input, out bool output)
    {
        output = !input;
        return true;
    }
}
```

### Data processor with two identical inputs

```C#
/// <summary>
/// Produces a logical AND of two values
/// </summary>
public sealed class AndGate : DualInFilter<bool, bool>
{
    protected override bool ProcessData(bool input1, bool input2, out bool output)
    {
        output = input1 && input2;
        return true;
    }
}
```

### Data converter

```C#
/// <summary>
/// Casts <see cref="System.String"/> to <see cref="System.Int32"/>
/// </summary>
public sealed class TypeCastFilter : DataFilter<string, int>
{
    protected override bool ProcessData(string input, out int output)
    {
        return Int32.TryParse(input, out output);
    }
}
```
