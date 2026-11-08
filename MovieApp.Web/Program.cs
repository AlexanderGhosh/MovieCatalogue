using Agent.Agentic.Abstractions.Extensions;
using Blazored.Modal;
using BlobStorage.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using MovieApp.Web.Agents;
using MovieApp.Web.Components;
using MovieApp.Web.Services.Abstractions;
using MovieApp.Web.Services.Implementations;
using OllamaSharp;

namespace MovieApp.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
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


            builder.Services.AddScoped<IChatClient>(s =>
            {
                var ollama = new OllamaApiClient(uriString: "http://localhost:11434/", "llama3.2:1b");
                return ollama;

            });
            builder.Services.AddKeyedAgent<VideoDescriptionAgent>();
            builder.Services.AddKeyedAgent<GeneralAgent>();

            builder.Services.AddControllers();
            builder.Services.AddHttpClient<VideoDescriptionAgent>();

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

            using (var scope = app.Services.CreateAsyncScope())
            {

                //    var names = new string[]
                //    {
                //"11|:Ghost Busters:Ghost Busters",
                //"59|:Les Misérables:Les Misérables Cinema",
                //"1269|Star Trek The Next Generation:Star Trek TNG S7:E1 Decent Part II",
                //"1353|Star Trek Deep Space Nine:Star Trek DS9 S3:E14 Heart of Stone",
                //"1422|Star Trek Deep Space Nine:Star Trek DS9 S6:E5 Favour the Bold",
                //"1425|Star Trek Deep Space Nine:Star Trek DS9 S6:E8 Resurrection",
                //"2585|Battlestar Galactica:Season 4:E6 Faith",
                //"2587|Battlestar Galactica:Season 4:E8 Sine Qua Non"
                //    };
                //    var controller = ActivatorUtilities.CreateInstance<VideoController>(scope.ServiceProvider);
                //    var i = 0;
                //    foreach (var name in names)
                //    {
                //        var split = name.Split('|');
                //        var id = int.Parse(split[0]);
                //        var n = split[1];
                //        var res = await controller.PutVideoDescription(id, null, true, n);
                //        if (res is not OkResult)
                //        {
                //            _ = 0;
                //        }
                //        Console.WriteLine($"[{++i}/{names.Length}] {n} Done");
                //    }
            }

            app.Run();
        }
    }
}
