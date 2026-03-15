using System.Net;

namespace Lab2
{
    internal static class Logger
    {
        private static string _outputFile = string.Empty;

        public static void Init()
        {
            _outputFile = $"logs/{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.log";
        }

        public static void Log(string message)
        {
            message += "\n";
            if (!Directory.Exists("logs"))
            {
                Directory.CreateDirectory("logs");
            }

            File.AppendAllText(_outputFile, message);
            Console.Write(message);
        }
    }

    internal class Program
    {
        private readonly static Dictionary<string, byte[]> _cache = [];
        private static volatile int _tasksCounter = 0;
        private static volatile bool _stop = false;
        private static HttpListener? listener;

        public static string CreateLogMessage(HttpListenerRequest request, HttpListenerResponse response)
        {
            return $"{DateTime.Now}: Request [{response.StatusCode}] '{request.Url}' from {request.RemoteEndPoint.Address}";
        }

        public static byte[]? ReadFile(string path)
        {
            if (!File.Exists(path)) return null;

            if (!_cache.TryGetValue(path, out byte[]? result))
            {
                result = File.ReadAllBytes(path);
                _cache[path] = result;
            }

            return result;
        }

        public static async Task HandleRequest(HttpListenerContext context)
        {
            Interlocked.Increment(ref _tasksCounter);

            try
            {
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;

                // Construct a response.
                if (request.Url == null) return;
                byte[]? responseBuffer = ReadFile($"static{request.Url.AbsolutePath}");
                if (responseBuffer == null)
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.Close();
                    Logger.Log(CreateLogMessage(request, response));
                    Interlocked.Decrement(ref _tasksCounter);
                    return;
                }

                // Get a response stream and write the response to it.
                if (request.AcceptTypes?.Length > 0)
                    response.ContentType = request.AcceptTypes[0];
                response.ContentLength64 = responseBuffer.Length;
                using Stream output = response.OutputStream;
                output.Write(responseBuffer, 0, responseBuffer.Length);
                output.Close();

                Logger.Log(CreateLogMessage(request, response));
            }
            finally
            {
                Interlocked.Decrement(ref _tasksCounter);
            }
        }

        public static async Task ServerListener(string[] prefixes)
        {
            if (!HttpListener.IsSupported)
            {
                return;
            }

            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException(null, nameof(prefixes));

            listener = new();
            foreach (string s in prefixes)
            {
                listener.Prefixes.Add(s);
            }
            listener.Start();
            Console.WriteLine("Listening...");

            while (listener.IsListening && !_stop)
            {
                try
                {
                    HttpListenerContext context = await listener.GetContextAsync();
                    _ = Task.Run(() => HandleRequest(context));
                }
                catch (HttpListenerException) // listener.Stop()
                {
                    break;
                }
            }

            Console.WriteLine("Wait tasks finish");
            while (_tasksCounter > 0)
            {
                await Task.Delay(10);
            }
            Console.WriteLine("All tasks finished");
            listener.Close();
        }

        static async Task Main(string[] args)
        {
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;
                _stop = true;
                listener?.Stop();
            };

            Logger.Init();
            await ServerListener([
                "http://localhost:8080/"
            ]);
        }
    }
}
