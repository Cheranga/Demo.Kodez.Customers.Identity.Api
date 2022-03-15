using System;
using Azure.Identity;
using Demo.Kodez.Customers.Identity.Api.Features.CreateCustomer.Models;
using Demo.Kodez.Customers.Identity.Api.Features.CreateCustomer.Services;
using Demo.Kodez.Customers.Identity.Api.Features.UpdateCustomer.Models;
using Demo.Kodez.Customers.Identity.Api.Features.UpdateCustomer.Services;
using Demo.Kodez.Customers.Identity.Api.Infrastructure.DataAccess;
using Demo.Kodez.Customers.Identity.Api.Shared;
using Demo.Kodez.Customers.Identity.Api.Shared.Services;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.FeatureManagement;
using Microsoft.OpenApi.Models;

namespace Demo.Kodez.Customers.Identity.Api
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
            services
                .AddAzureAppConfiguration()
                .AddFeatureManagement();

            RegisterServices(services);
            RegisterValidators(services);
            RegisterAzureClients(services, Configuration);
            RegisterResponseBuilders(services);

            services.AddControllers();
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = "Demo.Kodez.Customers.Identity.Api", Version = "v1"}); });
        }

        private void RegisterResponseBuilders(IServiceCollection services)
        {
            services.AddSingleton<IResponseBuilder<UpdateCustomerRequest, Result>, UpdateCustomerResponseBuilder>();
            services.AddSingleton<IResponseBuilder<CreateCustomerRequest, Result>, CreateCustomerResponseBuilder>();
        }

        private void RegisterServices(IServiceCollection services)
        {
            services.AddScoped<ICreateCustomerService, CreateCustomerService>();
            services.AddScoped<IUpdateCustomerService, UpdateCustomerService>();

            services.AddScoped<ICommandHandler<CreateCustomerCommand>, CreateCustomerCommandHandler>();
            services.AddScoped<ICommandHandler<UpdateCustomerCommand>, UpdateCustomerCommandHandler>();

            services.AddScoped<IQueryHandler<GetCustomerQuery, CustomerDataModel>, GetCustomerQueryHandler>();
        }

        private void RegisterAzureClients(IServiceCollection services, IConfiguration configuration)
        {
            services.AddAzureClients(builder =>
            {
                var config = new TableConfig();
                configuration.GetSection(nameof(TableConfig)).Bind(config);

                builder.AddTableServiceClient(new Uri(config.Connection)).WithCredential(new DefaultAzureCredential());
            });
        }

        private void RegisterValidators(IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(typeof(ModelValidatorBase<>).Assembly);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Demo.Kodez.Customers.Identity.Api v1"));
            }

            app.UseAzureAppConfiguration();
            
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}