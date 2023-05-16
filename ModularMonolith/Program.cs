using Autofac;
using Autofac.Extensions.DependencyInjection;
using ModularMonolith;
using Module_A;
using Module_B;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(c =>
{
    c.AddModule<ModuleA>();
    c.AddModule<ModuleB>();
});

var app = builder.Build();

app.MapControllers();

app.UseRouting();

app.Run();
