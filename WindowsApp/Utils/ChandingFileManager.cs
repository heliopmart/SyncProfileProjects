namespace WindowsApp.Utils{
    public class FileExpirationManager
    {
        private static readonly List<FileEntry> FileEntries = new();
        private static readonly TimeSpan ExpirationTime = TimeSpan.FromSeconds(10); // Define o tempo de expiração

        // Classe para armazenar o caminho e o DateTime
        private class FileEntry
        {
            public required string FilePath { get; set; }
            public DateTime Timestamp { get; set; }
        }

        /// <summary>
        /// Adiciona um arquivo à lista com o timestamp atual.
        /// </summary>
        /// <param name="filePath">O caminho do arquivo a ser adicionado.</param>
        public static void AddFile(string filePath)
        {
            FileEntries.Add(new FileEntry
            {
                FilePath = filePath,
                Timestamp = DateTime.Now
            });

            Console.WriteLine($"Arquivo adicionado: {filePath}, Timestamp: {DateTime.Now}");
        }

        /// <summary>
        /// Verifica se um arquivo expirou. Se expirou, remove da lista.
        /// </summary>
        /// <param name="filePath">O caminho do arquivo a ser verificado.</param>
        /// <returns>True se o arquivo não expirou, False se expirou e foi removido.</returns>
        public static bool CheckFile(string filePath)
        {
            var fileEntry = FileEntries.Find(entry => entry.FilePath == filePath);

            if (fileEntry == null)
            {
                Console.WriteLine($"Arquivo não encontrado: {filePath}");
                return false;
            }

            if (DateTime.Now - fileEntry.Timestamp > ExpirationTime)
            {
                FileEntries.Remove(fileEntry);
                Console.WriteLine($"Arquivo expirado e removido: {filePath}");
                return false;
            }

            Console.WriteLine($"Arquivo ainda válido: {filePath}");
            return true;
        }
    }

}