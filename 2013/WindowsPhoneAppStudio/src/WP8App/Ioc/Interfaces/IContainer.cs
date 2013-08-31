
namespace WPAppStudio.Ioc.Interfaces
{
    public interface IContainer
    {
        T Resolve<T>();
    }
}
