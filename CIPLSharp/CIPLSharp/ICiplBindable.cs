namespace CIPLSharp
{
    public interface ICiplBindable : ICiplCallable
    {
        public ICiplBindable Bind(CiplInstance instance);
    }
}