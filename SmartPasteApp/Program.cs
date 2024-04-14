using Microsoft.SemanticKernel;
using SmartPasteApp.Components;
using SmartPasteApp.Plugins;
using SmartPasteLib;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<CustomerSearchPlugin>();
builder.Services.AddSingleton(sp =>
    KernelPluginFactory.CreateFromObject(
        sp.GetRequiredService<CustomerSearchPlugin>()));
builder.Services.AddSmartPaste();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
