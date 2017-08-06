namespace AcrylecSkeleton.MVC
{
    /// <summary>
    /// Base class for model MVC objects.
    /// </summary>
    public abstract class Model : Element
    {

    }

    /// <summary>
    /// Base class for generic model MVC objects.
    /// </summary>
    /// <typeparam name="A"></typeparam>
    public class Model<A> : Model where A : Application
    {
        public new A App { get { return (A)base.App; } }
    }
}