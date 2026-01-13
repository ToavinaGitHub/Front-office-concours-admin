using Front_Office_Concours_Admin.Models;
using Front_Office_Concours_Admin.Repository;
using Front_Office_Concours_Admin.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// 🔗 Injection connection string
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

builder.Services.AddScoped<IAnnonceRepository, AnnonceRepository>();
builder.Services.AddScoped<CandidatureRepository>();
builder.Services.AddScoped<TypeContratRepository>();

builder.Services.AddSingleton<ElasticSearchService>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2); 
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var repo = scope.ServiceProvider.GetRequiredService<IAnnonceRepository>();
    var elastic = scope.ServiceProvider.GetRequiredService<ElasticSearchService>();

    int page = 1, pageSize = 100;
    AnnoncePagedViewModel annoncesBatch;

    do
    {
        annoncesBatch = repo.GetPagedAnnonces(page, pageSize);
        foreach (var annonce in annoncesBatch.Annonces)
        {
            elastic.IndexAnnonce(annonce); 
        }
        page++;
    } while (annoncesBatch.Annonces.Count > 0);
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Annonces}/{action=Index}/{id?}");

app.Run();