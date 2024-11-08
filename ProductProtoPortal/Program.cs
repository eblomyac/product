using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using ProtoLib.Managers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();

ProtoLib.ServiceInjector.Inject(builder.Services);
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
  //  app.UseSwagger();
  //  app.UseSwaggerUI();
}

string downloadDir = Path.Combine(Environment.CurrentDirectory, "download");
Directory.CreateDirectory(downloadDir);
app.UseHttpsRedirection();
app.UseRouting();
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(downloadDir),
    RequestPath = new PathString("/download")
});
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(TechCardManager.rootImagesDir),
    RequestPath = new PathString("/CustomImage")
});
app.MapControllers();


app.MapFallbackToFile("index.html");


app.Run();