namespace System.Web.TestUtil {
    public delegate void GenericDelegate();

    public delegate TReturnType GenericDelegate<TReturnType>();
    public delegate TReturnType GenericDelegate<TReturnType, U>(U u);
    public delegate TReturnType GenericDelegate<TReturnType, U, V>(U u, V v);
    public delegate TReturnType GenericDelegate<TReturnType, U, V, W>(U u, V v, W w);
    public delegate TReturnType GenericDelegate<TReturnType, U, V, W, X>(U u, V v, W w, X x);
    public delegate TReturnType GenericDelegate<TReturnType, U, V, W, X, Y>(U u, V v, W w, X x, Y y);
    public delegate TReturnType GenericDelegate<TReturnType, U, V, W, X, Y, Z>(U u, V v, W w, X x, Y y, Z z);

    public delegate void GenericVoidDelegate<T>(T t);
    public delegate void GenericVoidDelegate<T, U>(T t, U u);
    public delegate void GenericVoidDelegate<T, U, V>(T t, U u, V v);
    public delegate void GenericVoidDelegate<T, U, V, W>(T t, U u, V v, W w);
    public delegate void GenericVoidDelegate<T, U, V, W, X>(T t, U u, V v, W w, X x);
    public delegate void GenericVoidDelegate<T, U, V, W, X, Y>(T t, U u, V v, W w, X x, Y y);
    public delegate void GenericVoidDelegate<T, U, V, W, X, Y, Z>(T t, U u, V v, W w, X x, Y y, Z z);
}
