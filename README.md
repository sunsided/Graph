# Graph / Dataflow

**Graph** is a .NET library for parallel filter-based pipeline/dataflow processing.

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

## Example data processor (filter) with a single input

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

## Example data processor (filter) with two identical inputs

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

## Example data converter (filter)

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
