using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using System;

namespace CastleWindsorFindAllImplementations
{
    class Program
    {
        static void Main(string[] args)
        {
            IWindsorContainer container = new WindsorContainer();
            container.Kernel.Resolver.AddSubResolver(new ArrayResolver(container.Kernel));
            container.Register(AllTypes.Pick().FromAssembly(typeof(ApplicationRunner).Assembly)
                    .WithService.FirstInterface());

            IApplicationRunner runner = container.Resolve<IApplicationRunner>();
            runner.Run();

            Console.ReadLine();
        }
    }
}
