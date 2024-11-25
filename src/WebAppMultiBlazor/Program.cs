using WebAppMultiBlazor.Areas.Admin;
using WebAppMultiBlazor.Areas.Web;

// ---- SETUP
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorComponents()
	.AddInteractiveWebAssemblyComponents();

// --- START
var app = builder.Build();


// Configure the HTTP request pipeline.
app.MapStaticAssets();

// Global "api" endpoint
app.MapGet("/api/weatherforecast",
	() =>
	{
		string[] summaries =
		[
			"Freezing",
			"Bracing",
			"Chilly",
			"Cool",
			"Mild",
			"Warm",
			"Balmy",
			"Hot",
			"Sweltering",
			"Scorching",
		];
		var forecast = Enumerable.Range(1, 5).Select(index =>
				new WeatherForecast(
					DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
					Random.Shared.Next(-20, 55),
					summaries[Random.Shared.Next(summaries.Length)]
				))
			.ToArray();
		return forecast;
	});

// Admin
app.MapWhen(ctx => IsAdminPage(ctx),
	adminApp =>
	{
		adminApp.UsePathBase("/admin");

		// Configure the HTTP request pipeline.
		if (app.Environment.IsDevelopment())
		{
			adminApp.UseWebAssemblyDebugging();
		}
		else
		{
			adminApp.UseExceptionHandler("/Error", true);
			adminApp.UseHsts();
		}

		adminApp.UseBlazorFrameworkFiles("/admin");

		adminApp.UseStaticFiles("/admin");
		adminApp.UseStaticFiles();

		adminApp.UseRouting();
		adminApp.UseAntiforgery();
		adminApp.UseEndpoints(endpoints =>
		{
			endpoints.MapStaticAssets();
			endpoints.MapRazorComponents<AdminApp>()
				.AddInteractiveWebAssemblyRenderMode()
				.AddAdditionalAssemblies(typeof(WebAppMultiBlazor.Admin._Imports).Assembly);
		});
	});

// Main web
app.MapWhen(ctx => ctx.Request.Path.StartsWithSegments("/"), // not sure if needed?
	defaultApp =>
	{
		defaultApp.UsePathBase("/web");

		// Configure the HTTP request pipeline.
		if (app.Environment.IsDevelopment())
		{
			defaultApp.UseWebAssemblyDebugging();
		}
		else
		{
			defaultApp.UseExceptionHandler("/Error", true);
			defaultApp.UseHsts();
		}

		defaultApp.UseBlazorFrameworkFiles("/");

		defaultApp.UseStaticFiles();

		defaultApp.UseRouting();
		defaultApp.UseAntiforgery();
		defaultApp.UseEndpoints(endpoints =>
		{
			endpoints.MapStaticAssets();
			endpoints.MapRazorComponents<WebApp>()
				.AddInteractiveWebAssemblyRenderMode()
				.AddAdditionalAssemblies(typeof(WebAppMultiBlazor.Web._Imports).Assembly);
		});
	});


// TODO: another app base on subdomain?


app.Run();

bool IsAdminPage(HttpContext httpContext)
{
	// could be base on subdomain too ???
	return httpContext.Request.Path.StartsWithSegments("/admin");
}

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
	public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}