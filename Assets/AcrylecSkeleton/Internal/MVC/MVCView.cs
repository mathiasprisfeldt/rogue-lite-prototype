namespace AcrylecSkeleton.MVC
{
    /// <summary>
    /// Base class for view MVC objects.
    /// </summary>
    public abstract class View : Element
    {

    }


    /// <summary>
    /// Base class for generic view MVC objects.
    /// </summary>
    /// <typeparam name="A"></typeparam>
    public class View<A> : View where A : Application
    {
        new public A App { get { return (A)base.App; } }
    }
}