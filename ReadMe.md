## AspNetCore.ControllerInjection

The default service provider from Microsoft support only Constructor Injection which is great and fast, However in large application it become hard to design the class hierarchy because of this limitation of the service provider. So this package attempt to solve this problem by providing a controller activator which extend the service provider - in the matter of speaking - for MVC controllers to support field, property and method injection in a simple way.


### How to

```cs
// register the services in the Startup.ConfigureServices
services.AddTransient<IControllerActivator, InjectableControllerActivator>();
services.AddSingelton<IControllerMetadataProvider, DefaultControllerMetadataProvider>();
```

The first line sets the new controller activator which create/release the controller.

The second line register the provider which cache the controller metadata required to do the injection later.


### The attributes

Once you register the services all you need is to use the attributes `Inject` and `InjectOrder` to direct the DI process.

The `Inject` attribute used to mark a field, property or a method that it will be used for injection, this attribute have two optional properties:

1. `IsRequired` default to false, when set and the service provider cannot find the implementation for a service the injector will throw exception.
2. `ServiceType` use it to set the service type as you specified in the DI registeration.

The `InjectOrder` attribute used to specify the order in which the injection will take place, this attribute have only one property:

1. `Order` the order of injection for the element it tagged.

If the `InjectOrder` didn't specified in the element the default order will be `int.MaxValue` and is there are multiple elements with same order, the injection order is not specified.


#### Before we go on

Please note that the default metadata provider `DefaultControllerMetadataProvider` gets all members of controller no matter their visibility and under specific condition:

* Fields: must be read-write field.
* Properties: must have write access.
* Methods: must have a return type `void` with one parameter at least and all parameters of reference type.

#### Fields

let's assume we have the registered the default implementation of `IHttpContextAccessor`, to inject it into a field, use the following code:

```cs
[Inject]
private IHttpContextAccessor _httpAccessor;
```

if you didn't register the `IHttpContextAccessor` the value of `_httpAccessor` will be null, to force check the existance of the service just pass true to `Inject` attribute to signal that the inject is required

```cs
[Inject(true)]
private IHttpContextAccessor _httpAccessor;
```

if you want the field type to be of the implementation type not the service type, then pass the service type to the `Inject` attribute

```cs
// services.AddTransient<IFoo, Foo>();

[Inject(typeof(IFoo))]
private Foo _foo;
```

and if you want to throw exception if service is not registered, just pass `true` as a second argument to `Inject` attribute

```cs
[Inject(typeof(IFoo))]
private Foo _foo;
```

#### Properties

all roles for Fields apply here


#### Methods

To inject a method only mark it with `Inject` attribute, the only supported argument for methods is `IsRequired` which act as a default behavior for the injection of the parameters.

The parameters themselves have the rules same as Fields.

```cs
private void Initialize(IHttpContextAccessor httpAccessor, [Inject(typeof(IFoo))] Foo foo)
{
    _httpAccessor = httpAccessor;
    _foo = foo;
}
```
