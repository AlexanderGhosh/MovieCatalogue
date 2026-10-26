using Blazored.Modal;
using BlobStorage.Extensions;
using Microsoft.EntityFrameworkCore;
using MovieApp.Web.Components;
using MovieApp.Web.Services.Abstractions;
using MovieApp.Web.Services.Implementations;

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

            builder.Services.AddDbContextFactory<MovieAppContext>(o =>
            {
                o.UseSqlServer(builder.Configuration.GetConnectionString(nameof(MovieAppContext)));
            });
#if DEBUG
            builder.Services.AddLocalBlobStorage("C:\\Users\\AGWDW\\Desktop\\Programing\\C#\\MovieCatalogue\\Movies");
#else
            builder.Services.AddLocalBlobStorage("./Movies");
#endif
            builder.Services.AddScoped<IMovieService, MovieService>();
            builder.Services.AddScoped<IDragDropService, DragDropService>();
            builder.Services.AddScoped<IAudioPlayer, AudioPlayer>();

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
