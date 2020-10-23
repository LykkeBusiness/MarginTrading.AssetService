using Autofac;
using Autofac.Builder;

namespace MarginTrading.AssetService.Extensions
{
    public static class ContainerBuilderExtensions
    {
        public static IRegistrationBuilder<TType, ConcreteReflectionActivatorData, SingleRegistrationStyle>
            RegisterSingletonIfNotRegistered<TType, TInterface>(this ContainerBuilder builder)
        {
            return builder.RegisterType<TType>()
                .As<TInterface>()
                .SingleInstance()
                .IfNotRegistered(typeof(TInterface));
        }
    }
}