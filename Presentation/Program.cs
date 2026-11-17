using Presentation.Composition;
using Presentation.Console;

// Prvi argument: nacin cuvanja ("in-memory" ili "json"); drugi: logger ("file" ili "console").
string rezim = args.Length > 0 ? args[0] : "in-memory";
string logger = args.Length > 1 ? args[1] : "file";
AppContainer container = AppContainer.Create(rezim, logger);
new ConsoleMenu(container.Io, container).Run();
