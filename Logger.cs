
namespace Engine
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
            Debug.Log();
        }

        public void Log(object message)
        {
            Debug.Log(message, context, Debug.Options.Normal);
        }

        public void Warn(object message)
        {
            Debug.Log(message, context, Debug.Options.Warning);
        }

        public void Error(object message)
        {
            Debug.Log(message, context, Debug.Options.Error);
        }

        public void Success(object message)
        {
            Debug.Log(message, context, Debug.Options.Success);
        }
    }
}
