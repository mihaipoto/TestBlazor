using Syncfusion.Blazor;
using TestBlazor;
using TestBlazor.Components;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Authentication.Negotiate;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();

builder.Services.AddSyncfusionBlazor();
builder.Services.AddSingleton<ITrafficClassifier, TrafficClassifier>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<ITrafficClassifier>());

builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
    .AddNegotiate()
    .AddCertificate(options =>
    {
        options.AllowedCertificateTypes = CertificateTypes.All;
        options.ValidateValidityPeriod = false;
        options.RevocationMode = X509RevocationMode.NoCheck;
        options.Events = new CertificateAuthenticationEvents
        {
            OnCertificateValidated = context =>
            {
                var claims = new[]
                {
                    new System.Security.Claims.Claim(
                        System.Security.Claims.ClaimTypes.NameIdentifier,
                        context.ClientCertificate.Subject,
                        System.Security.Claims.ClaimValueTypes.String,
                        context.Options.ClaimsIssuer),
                };

                context.Principal = new System.Security.Claims.ClaimsPrincipal(
                    new System.Security.Claims.ClaimsIdentity(claims, context.Scheme.Name));
                context.Success();

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy;

    options.AddPolicy("CertificateRequired", policy =>
    {
        policy.AuthenticationSchemes = new[] { CertificateAuthenticationDefaults.AuthenticationScheme };
        policy.RequireAuthenticatedUser();
    });

    options.AddPolicy("NegotiateRequired", policy =>
    {
        policy.AuthenticationSchemes = new[] { NegotiateDefaults.AuthenticationScheme };
        policy.RequireAuthenticatedUser();
    });

});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapGet("/w", (HttpContext context) =>
{
    return context.User.Identity?.Name ?? "No username found";
})
.RequireAuthorization("NegotiateRequired");

app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();

app.Run();
