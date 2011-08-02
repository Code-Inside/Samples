namespace Client
{
    using System;
    using System.ServiceModel;
    using System.Threading;
    using Castle.Core;
    using Castle.DynamicProxy;
    using System.Reflection;
    using Castle.Facilities.WcfIntegration;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using Contracts;

    class Program
    {

        static void Main(string[] args)
        {
            IWindsorContainer container = new WindsorContainer();

            container.Register(Component.For(typeof(WcfProxyInterceptor<IService1>)));

            container.Register(Component.For(typeof (IFoo)).ImplementedBy(typeof (Foo)));
            container.Register(Component.For(typeof(IBar)).ImplementedBy(typeof(Bar)));

            container.Register(Component.For<IService1>().Interceptors(InterceptorReference.ForType<WcfProxyInterceptor<IService1>>()).Anywhere);


            var test = (IFoo)container.Resolve(typeof (IFoo));
            test.Do();
            test.Do();
            test.Do();
            test.Do();
            test.Do();

            var testBar = (IBar)container.Resolve(typeof(IBar));
            testBar.Do();
            
            Console.ReadLine();
        }
    }

    public interface IFoo
    {
        void Do();
    }

    public interface IBar
    {
        void Do();
    }

    public class Bar : IBar
    {
        public IService1 Service1 { get; set; }
        
        public Bar(IService1 srv)
        {
            this.Service1 = srv;
        }

        public void Do()
        {
            Console.WriteLine(this.Service1.GetData(999));
        }
    }

    public class Foo : IFoo
    {
        public IService1 Service1 { get; set; } 
        public Foo(IService1 srv)
        {
            this.Service1 = srv;
        }

        public void Do()
        {
            Console.WriteLine(this.Service1.GetData(999));
        }
    }

    public class WcfProxyInterceptor<TService> : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            var backendWsHttpBinding = new BasicHttpBinding();
            var address = new EndpointAddress("http://localhost:64013/Service1.svc");

            var channelFactory = new ChannelFactory<TService>(backendWsHttpBinding, address);

            IClientChannel channel = channelFactory.CreateChannel() as IClientChannel;

            if (channel != null)
            {
                try
                {
                    var response = invocation.Method.Invoke(channel, invocation.Arguments);
                    invocation.ReturnValue = response;
                    channel.Close();
                }
                catch (Exception e  )
                {   
                    channel.Abort();
                    Console.WriteLine("Error...");
                }
            }
        }
    }    
}
