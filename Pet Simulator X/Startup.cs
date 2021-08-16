using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using DiscordRPC;
using System.Text.Json;
using System.IO;

namespace Pet_Simulator_X
{  
    public class Startup
    {
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            bool connected = false;
            Console.Title = "Pet Simulator X Rich Presence";
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[STATUS] Pet Simulator X Rich Presence Initialized.");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("[INFO] Please execute the script in your exploit. Go to http://localhost:9090 to get the script if you haven't already.");

            DiscordRpcClient client = new DiscordRpcClient("876683468015292486");
            Assets assets = new Assets()
            {
                LargeImageKey = "gameicon",
                LargeImageText = "Pet Simulator X"
            };
            bool rpcReady = false;
            client.OnReady += (sender, e) =>
            {
                rpcReady = true;
                client.SetPresence(new RichPresence
                {
                    State = "Waiting for data",
                    Assets = assets,
                    Timestamps = new Timestamps(DateTime.UtcNow)
                });
            };
            client.Initialize();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    context.Response.ContentType = "text/html";
                    await context.Response.WriteAsync(@"
                    <body style='background: #202020; display: flex; flex-direction: column; align-items: center; justify-content: center; width: 100vw; height: 100vh; overflow: hidden;'>
                        <h1 style='font-family: arial; color: white; text-align: center; margin-bottom: -1;'>Pet Simulator X Rich Presence</h1>
                        <h1 style='font-family: arial; color: white; text-align: center;'>By VIPER#0001</h1>
                        <a style='font-family: arial; color: white; background-color: #676767; padding-top: 1vh; padding-bottom: 1vh; padding-right: 5vw; padding-left: 5vw; border-radius: 10px; text-decoration: none;' href='https://viper.tools/PetSimulatorXRPC.lua'>Get Script</a>
                    </body>");
                });

                endpoints.MapPost("/UpdatePresence", async context =>
                {
                    if (!connected)
                    {
                        connected = true;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("[CLIENT] Roblox Client Connected!");
                    }
                    JsonElement json = (await JsonDocument.ParseAsync(context.Request.Body)).RootElement;
                    if (rpcReady)
                    {
                        client.UpdateDetails(json.GetProperty("Area").GetString());
                        client.UpdateState(json.GetProperty("Data").GetString());
                    }

                    await context.Response.WriteAsync("OK");
                });
            });
        }
    }
}
