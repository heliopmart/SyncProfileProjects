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
                    return IsPathIgnored(NormalizePath(path));
                case "add":
                    return IgnorePath(NormalizePath(path));
                default:
                    return false;
            }
        }

        private static bool IgnorePath(string path)
        {
            var expirationTime = DateTime.Now.AddSeconds(6);;
            IgnoredPaths[path] = expirationTime;

            // Iniciar uma tarefa para remover o caminho após 5 segundos
            _ = Task.Run(() =>
            {
                Thread.Sleep(6000);
                RemoveIgnoredPath(path);
            });

            return true;
        }

        private static void RemoveIgnoredPath(string path)
        {
            Console.WriteLine("Token removido ou espirado");
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

        private static string NormalizePath(string path)
        {
            return Path.GetFullPath(path)
                .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                .ToLowerInvariant(); // Converte para letras minúsculas
        }
    }
}
