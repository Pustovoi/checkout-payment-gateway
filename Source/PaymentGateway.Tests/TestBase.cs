using Autofac;
using Autofac.Extras.Moq;
using AutoMapper;
using PaymentGateway.Services;

namespace PaymentGateway.Tests
{
	public class TestBase
	{
		protected AutoMock GetMockWithAutoMapper()
		{
			return AutoMock.GetLoose((builder) =>
			{
				builder.Register(
						c => new MapperConfiguration(cfg => { cfg.AddProfiles(new[] { new MappingProfile() }); }))
					.AsSelf()
					.SingleInstance();

				//register  mapper
				builder.Register(
						c => c.Resolve<MapperConfiguration>().CreateMapper(c.Resolve))
					.As<IMapper>().InstancePerLifetimeScope();
			});
		}
	}
}
