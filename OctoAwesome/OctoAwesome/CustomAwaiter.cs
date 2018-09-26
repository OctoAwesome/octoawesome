using System.Threading;

namespace OctoAwesome
{
    public class CustomAwaiter
    {
        private ISerializable result;
        private ManualResetEventSlim manualReset;

        public CustomAwaiter() => manualReset = new ManualResetEventSlim(false);

        public T WaitOn<T>() where T : ISerializable
        {
            manualReset.Wait();
            return (T)result;
        }

        public void SetResult(ISerializable result)
        {
            this.result = result;
            manualReset.Set();
        }
    }

    public class CustomAwaiter<T> : CustomAwaiter where T : ISerializable
    {
        public T Result => WaitOn<T>();
    }
}