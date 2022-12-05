using Autofac;
using BookOnline.Borrowing.Api.Infrastucture.Commands;
using BookOnline.Borrowing.Api.Infrastucture.Repositories;
using RabbitMQEventBus;
using System.Reflection;

namespace BookOnline.Borrowing.Api.Infrastucture.AutofacModules;

public class ApplicationModule
    : Autofac.Module
{

    public string QueriesConnectionString { get; }

    public ApplicationModule(string qconstr)
    {
        QueriesConnectionString = qconstr;

    }

    protected override void Load(ContainerBuilder builder)
    {
        //builder.RegisterType<MemberRepository>()
        //    .As<IMemberRepository>()
        //    .InstancePerLifetimeScope();

        //builder.RegisterType<BorrowRepository>()
        //    .As<IBorrowRepository>()
        //    .InstancePerLifetimeScope();


        builder.RegisterAssemblyTypes(typeof(CreateBorrowCommandHandler).GetTypeInfo().Assembly)
            .AsClosedTypesOf(typeof(IEventHandler<>));

    }
}
