using Microsoft.EntityFrameworkCore;
using PROGPart2.Models;

var builder = WebApplication.CreateBuilder(args);


// Register CMCSContext with the DI container
builder.Services.AddDbContext<CMCSContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CMCSDatabase")));

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
