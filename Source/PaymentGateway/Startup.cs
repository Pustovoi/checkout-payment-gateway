using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.DataModel;
using PaymentGateway.Repositories;
using PaymentGateway.Services;

namespace PaymentGateway
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public ILifetimeScope AutofacContainer { get; private set; }


		// This method gets called by the runtime. Use this method to add services to the container.
		public IServiceProvider ConfigureServices(IServiceCollection services)
		{
			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

			services.AddOptions();

			var builder = new ContainerBuilder();
			builder.Populate(services);

			PopulateBuilder(builder);

			AutofacContainer = builder.Build();

			// this will be used as the service-provider for the application!
			return new AutofacServiceProvider(AutofacContainer);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseMvc();
		}

		private void PopulateBuilder(ContainerBuilder builder)
		{
			builder.RegisterType<PaymentRepository>().As<IPaymentRepository>();
			builder.RegisterType<PaymentService>().As<IPaymentService>();
			builder.RegisterType<BankServiceStub>().As<IBankService>();

			builder.RegisterType<PaymentGatewayDataContext>().AsSelf();

			builder.Register(c => new MapperConfiguration(cfg =>
			{
				cfg.AddProfiles(new [] {new MappingProfile()});
			})).AsSelf().SingleInstance();

			//register  mapper
			builder.Register(
					c => c.Resolve<MapperConfiguration>().CreateMapper(c.Resolve))
				.As<IMapper>().InstancePerLifetimeScope();
		}
	}
}
