# Graph / Dataflow

**Graph** is a .NET library for parallel filter-based pipeline/dataflow processing.

# Usage example

1. Create a data source

	```C#
	ISource source = new LogicEmitter();
	```

	(Note: For the ```LogicEmitter``` to actually do something you must call one of its ```EmitXXX()``` methods. This is just an example though.)

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
6. ???
7. Profit
