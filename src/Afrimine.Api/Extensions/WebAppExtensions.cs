using Microsoft.AspNetCore.HttpOverrides;

namespace Afrimine.Api.Extensions
{
    public static class WebAppExtensions
    {
        public static IApplicationBuilder UseApiMiddlewares(this WebApplication app,
                                                            IConfiguration configuration)
        {
            app.UseSwaggerDocsUI();
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            return app;
        }

        static WebApplication UseSwaggerDocsUI(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                options.SwaggerEndpoint("/swagger/v2/swagger.json", "v2");
            });

            return app;
        }
    }
}
