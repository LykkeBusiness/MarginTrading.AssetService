using Autofac;
using Lykke.Cqrs;
using Moq;

namespace MarginTrading.AssetService.Tests.AutofacModules
{
    public class TestCqrsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(new Mock<ICqrsEngine>().Object);
        }
    }
}