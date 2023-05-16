using ModularMonolith;
using Module_A;
using Module_B;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddModule<ModuleA>();
builder.Services.AddModule<ModuleB>();

var app = builder.Build();

app.MapControllers();

app.UseRouting();

app.Run();
