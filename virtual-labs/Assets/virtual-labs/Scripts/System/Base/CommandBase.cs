namespace Praxilabs
{
    /// <summary> Base class for command pattern, inherit from this if you want to use commands</summary>
    public abstract class CommandBase
    {
        public abstract void Execute();
        public virtual void StopExecuting() { }
    }
}