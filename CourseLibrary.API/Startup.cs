using AutoMapper;
using CourseLibrary.API.DbContexts;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace CourseLibrary.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(setupAction =>
            {
                setupAction.ReturnHttpNotAcceptable = true;
            }).AddXmlDataContractSerializerFormatters() // configured to return xml data when accept header is define to xml. JSON type is returned as default data-type.
              .ConfigureApiBehaviorOptions(setupAction => 
              {
                  setupAction.InvalidModelStateResponseFactory = context =>
                  {
                      // create a problem details object
                      var problemsDetailsFactory = context.HttpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>();

                      var problemDetails = problemsDetailsFactory.CreateValidationProblemDetails(context.HttpContext, context.ModelState);

                      // add additional info not added by default
                      problemDetails.Detail = "See the errors field for details.";
                      problemDetails.Instance = context.HttpContext.Request.Path;

                      // find out witch status code to use
                      var actionExecutingContext = context as Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext;

                      // if there are ModelState errors & all arguments were correctly
                      // found/parsed we are dealing with validation errors
                      if ((context.ModelState.ErrorCount > 0) && (actionExecutingContext?.ActionArguments.Count == context.ActionDescriptor.Parameters.Count))
                      {
                          problemDetails.Type = "https://courselibrary.com/modelvalidationproblem";
                          problemDetails.Status = StatusCodes.Status422UnprocessableEntity;
                          problemDetails.Title = "One or more validation erros occurred.";

                          return new UnprocessableEntityObjectResult(problemDetails)
                          {
                              ContentTypes = { "application/problem+json" }
                          };
                      }
                      // if one of the arguments wasn't correctly found / couldn't be parsed
                      // we're dealing with null/unparseable input

                      problemDetails.Status = StatusCodes.Status400BadRequest;
                      problemDetails.Title = "One or more erros on input occurred.";

                      return new BadRequestObjectResult(problemDetails)
                      {
                          ContentTypes = { "application/problem+json" }
                      };

                  };
              });

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            
            services.AddScoped<ICourseLibraryRepository, CourseLibraryRepository>();

            services.AddDbContext<CourseLibraryContext>(options =>
            {
                options.UseSqlServer(
                    @"Server=(localdb)\mssqllocaldb;Database=CourseLibraryDB;Trusted_Connection=True;");
            }); 
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler( appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An unexpect fault happened. Try again later.");
                    });
                });
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
