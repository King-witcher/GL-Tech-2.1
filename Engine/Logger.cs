
namespace GLTech
{
    public class Logger
    {
        private string context;

        public Logger(string context = null)
        {
            this.context = context;
        }

        public void Log()
        {
            Debug.LogTerminal();
        }

        public void Log(object message)
        {
            Debug.LogTerminal(message, context, Debug.Options.Normal);
        }

        public void Warn(object message)
        {
            Debug.LogTerminal(message, context, Debug.Options.Warning);
        }

        public void Error(object message)
        {
            Debug.LogTerminal(message, context, Debug.Options.Error);
        }

        public void Success(object message)
        {
            Debug.LogTerminal(message, context, Debug.Options.Success);
        }
    }
}
