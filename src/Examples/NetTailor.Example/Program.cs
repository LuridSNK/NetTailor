using Microsoft.Extensions.DependencyInjection;
using NetTailor;
using NetTailor.Example;
using NetTailor.Extensions;

// define service collection & configuration
var services = new ServiceCollection();
services.AddHttpClientProfile<ExampleClientProfile>();

var serviceProvider = services.BuildServiceProvider();

var dispatcher = serviceProvider.GetRequiredService<IHttpDispatcher>();
