using System.Collections.Concurrent;

namespace WindowsApp.Utils
{
    class IgnoreFiles
    {
        private static readonly ConcurrentDictionary<string, DateTime> IgnoredPaths = new ConcurrentDictionary<string, DateTime>();

        public static bool HandlePath(string path, string type)
        {
            switch (type)
            {
                case "get":
                    return IsPathIgnored(path);
                case "add":
                    return IgnorePath(path);
                default:
                    return false;
            }
        }

        private static bool IgnorePath(string path)
        {
            var expirationTime = DateTime.Now;
            IgnoredPaths[path] = expirationTime;

            // Iniciar uma tarefa para remover o caminho após 2 segundos
            _ = Task.Run(() =>
            {
                Thread.Sleep(2000);
                RemoveIgnoredPath(path);
            });

            return true;
        }

        private static void RemoveIgnoredPath(string path)
        {
            IgnoredPaths.TryRemove(path, out _);
        }

        private static bool IsPathIgnored(string path)
        {
            if (IgnoredPaths.TryGetValue(path, out var expirationTime))
            {
                // Verificar se o tempo de expiração já passou
                if (DateTime.Now > expirationTime)
                {
                    RemoveIgnoredPath(path);
                    return false;
                }
                return true;
            }
            return false;
        }
    }
}
