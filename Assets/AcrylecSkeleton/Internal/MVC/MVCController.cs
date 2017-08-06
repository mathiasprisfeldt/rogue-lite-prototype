namespace AcrylecSkeleton.MVC
{
    /// <summary>
    /// Base class for controller MVC objects.
    /// </summary>
    public abstract class Controller : Element
    {

    }


    /// <summary>
    /// Base class for generic controller MVC objects.
    /// </summary>
    /// <typeparam name="A"></typeparam>
    public class Controller<A> : Controller where A : Application
    {
        new public A App { get { return (A) base.App; } }
    }
}