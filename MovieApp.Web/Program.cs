using Blazored.Modal;
using BlobStorage.Extensions;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using MovieApp.Web.Components;
using MovieCatalogue;

namespace MovieApp.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddBlazoredModal();

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            builder.Services.AddDbContext<MovieAppContext>(o =>
            {
                o.UseSqlServer(builder.Configuration.GetConnectionString(nameof(MovieAppContext)));
            });
            builder.Services.AddLocalBlobStorage("C:\\Users\\AGWDW\\Desktop\\Programming\\C#\\MovieCatalogue\\Movies");
            builder.Services.AddScoped<IMovieService, MovieService>();

            builder.Services.AddControllers();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.MapControllers();

            app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
            app.UseHttpsRedirection();

            app.UseAntiforgery();

            app.MapStaticAssets();
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
