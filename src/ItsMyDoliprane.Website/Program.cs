using ItsMyDoliprane.Business;
using ItsMyDoliprane.Business.Extensions;
using ItsMyDoliprane.Repository;
using Microsoft.Net.Http.Headers;
using NLog;
using NLog.Web;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

try {
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddControllersWithViews();
    builder.Services.AddDolipraneBusiness();
    builder.Services.AddTransient<UsePersons>();
    builder.Services.AddTransient<UseDrugs>();
    builder.Services.AddTransient<UseMedications>();
    builder.Services.AddTransient<PersonRepository>();
    builder.Services.AddTransient<DrugRepository>();
    builder.Services.AddTransient<MedicationRepository>();

    // NLog: Setup NLog for Dependency injection
    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
    builder.Host.UseNLog();

    var app = builder.Build();

    app.UsePathBase(app.Configuration["path-base"]);

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment()) {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    if (app.Environment.IsDevelopment())
        app.UseStaticFiles();
    else
        app.UseStaticFiles(new StaticFileOptions {
            OnPrepareResponse = ctx => {
                ctx.Context.Response.Headers[HeaderNames.CacheControl] = "public,max-age=31536000,immutable";
            }
        });
        

    app.UseRouting();

    app.UseAuthorization();

    app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

    app.Run();
}
catch (Exception exception) {
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally {
    LogManager.Shutdown();
}
