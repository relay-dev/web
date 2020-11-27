<img src="https://i.imgur.com/fDu6Ncd.png" alt="Web" height="100" width="100">

# Web

[![Build status](https://ci.appveyor.com/api/projects/status/l1cmbnybohcnwvm7/branch/master?svg=true)](https://ci.appveyor.com/project/scottjferguson/web/branch/master)
[![NuGet Release](https://img.shields.io/nuget/v/relay.web.svg)](https://www.nuget.org/packages/Relay.Web/)
[![License](https://img.shields.io/github/license/relay-dev/web.svg)](https://github.com/relay-dev/web/blob/master/LICENSE)

## Introduction

Web is a set of base class libraries written using .NET Standard. They serve to consolidate shared code for web applications; particularly bootstrap and application configuration code. The packages are divided by the nature of the applications they intend to support.

Web depends on Core.Plugins, which can be found on [GitHub](https://github.com/relay-dev/core-plugins) and [NuGet](https://www.nuget.org/packages/Relay.Core.Plugins). There are examples of Startup.cs and Program.cs below in the Usage section which demonstrate how to configure your application to work with Web. The method calls to the framework (AddWebFramework() and UseWebFramework()) will internally call the equivalent Core.Plugin setup methods.

In fact, there is a hierarchy that exists in the Configuration types and builders where Core.Plugins is the lowest in the hierarchy, which is extended by Web, which is extended by Web.Rest and Web.AzureFunctions. This way, the more targeted packages can offer the features of all the packages lower than it.

Web is published to the main public NuGet feed (see section [Installation](#installation)).

## Getting Started

<a name="installation"></a>

### Installation

Follow the instructions below to install this NuGet package into your project:

#### .NET Core CLI

```sh
dotnet add package Relay.Web
dotnet add package Relay.Web.AzureFunctions
dotnet add package Relay.Web.Rest
```

#### Package Manager Console

```sh
Install-Package Relay.Web
Install-Package Relay.Web.AzureFunctions
Install-Package Relay.Web.Rest
```

### Latest releases

Latest releases and package version history can be found on [NuGet](https://www.nuget.org/packages/Relay.Web/).

## Build and Test

Follow the instructions below to build and test this project:

### Build

#### .NET Core CLI

```sh
dotnet build
```

### Test

#### .NET Core CLI

```sh
dotnet test
```

## Usage

The packages are divided by the nature of the applications they intend to support. The usage of each package is demonstrated here:

### Web

#### Startup.cs

Here is an example of how to configure a Startup class:

```c#
public class Startup
{
    private readonly WebConfiguration _webConfiguration;

    public Startup(IConfiguration configuration)
    {
        _webConfiguration = BuildWebConfiguration(configuration);
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddWebFramework(_webConfiguration);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseWebFramework(_restConfiguration, env);
    }

    private WebConfiguration BuildWebConfiguration(IConfiguration configuration)
    {
        return new WebConfigurationBuilder()
            .UseApplicationName("WebApplication")
            .UseApplicationConfiguration(configuration)
            .UseServiceLifetime(ServiceLifetime.Scoped)
            .UseCommandHandlersFromAssemblyContaining<CreateOrderHandler>()
            .UseMappersFromAssemblyContaining<AutoMappers>()
            .UseValidatorsFromAssemblyContaining<OrderValidator>()
            .UseWarmupTypesFromAssemblyContaining<Startup>()
            .Build();
    }
}
```

#### Program.cs

You don't need to do anything special with Program.cs. It just needs to call Startup. If you want to assume all the defaults, you are welcome to inherit from WebProgram. It exposes a CreateHostBuilder() method that makes standing up applications quick when you don't have much complexity.

```c#
public class Program : WebProgram<Startup>
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }
}
```

### Consuming Plugins

Once you've configured your application to use plugins, you can start requesting them from the constructor.

#### Business Operation example

Here is an example of a class that can run a typical business operation:

```c#
public class CreateOrderHandler : IRequestHandler<CreateOrderRequest, CreateOrderResponse>
{
    private readonly OrderContext _dbContext;
    private readonly IValidator<Order> _validator;
    private readonly IMapper _mapper;
    private readonly IEventClient _eventClient;

    public CreateOrderHandler(
        OrderContext dbContext,
        IValidator<Order> validator,
        IMapper mapper,
        IEventClient eventClient)
    {
        _dbContext = dbContext;
        _validator = validator;
        _mapper = mapper;
        _eventClient = eventClient;
    }

    public async Task<CreateOrderResponse> Handle(CreateOrderRequest request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request.Order, cancellationToken);

        OrderEntity orderEntity = _mapper.Map<OrderEntity>(request.Order);

        await _dbContext.AddAsync(orderEntity, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        await RaiseOrderCreatedEvent(orderEntity, cancellationToken);

        return new CreateOrderResponse
        {
            OrderId = orderEntity.OrderId
        };
    }

    private async Task RaiseOrderCreatedEvent(OrderEntity orderEntity, CancellationToken cancellationToken)
    {
        var e = new Event
        {
            EventType = "OrderCreated",
            Subject = "OrderCreated",
            Data = new OrderCreatedPayload
            {
                OrderId = orderEntity.OrderId,
                OrderDate = orderEntity.OrderDate
            }
        };

        await _eventClient.RaiseEventAsync(e, cancellationToken);
    }
}
```

### Web.AzureFunctions

#### Startup.cs

Here is an example of how to configure a Startup class:

```c#
public class Startup : FunctionsStartup
{
    private readonly AzureFunctionsConfiguration _azureFunctionsConfiguration;

    public Startup()
    {
        _azureFunctionsConfiguration = BuildAzureFunctionsConfiguration();
    }

    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddAzureFunctionsFramework<AccountDbContext>(_azureFunctionsConfiguration);

        builder.Services.AddSqlServerDatabaseCommander(_azureFunctionsConfiguration.Configuration);
    }

    private AzureFunctionsConfiguration BuildAzureFunctionsConfiguration()
    {
        return new ConfigurationBuilder()
            .AsAzureFunctionsConfiguration()
            .UseApplicationName("Account.EventHandlers")
            .UseFunctionsFromAssemblyContaining<Startup>()
            .AsEventHandler()
            .Build();
    }
}
```

### Web.Rest

#### Startup.cs

Here is an example of how to configure a Startup class:

```c#
public class Startup
{
    private readonly RestConfiguration _restConfiguration;

    public Startup(IConfiguration configuration)
    {
        _restConfiguration = BuildRestConfiguration(configuration);
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddRestFramework<AccountDbContext>(_restConfiguration);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRestFramework(_restConfiguration, env);
    }

    private RestConfiguration BuildRestConfiguration(IConfiguration configuration)
    {
        return new RestConfigurationBuilder()
            .UseConfiguration(configuration)
            .UseApplicationName("Account.API")
            .UseCommandHandlersFromAssemblyContaining<CreateAccountHandlerBase>()
            .UseMappersFromAssemblyContaining<AutoMappers>()
            .UseValidatorsFromAssemblyContaining<AccountValidator>()
            .UseWarmupTypesFromAssemblyContaining<LookupDataWarmupTask>()
            .Build();
    }
}
```

## Contribute

When contributing to this repository, please first discuss the change you wish to make via issue,
email, or any other method with the owners of this repository before making a change.
