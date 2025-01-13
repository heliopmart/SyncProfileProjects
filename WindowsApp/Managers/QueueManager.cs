using System.Collections.Concurrent;
using WindowsApp.Models;

namespace WindowsApp.Managers
{   
    public class QueueManager
    {
        private readonly ConcurrentQueue<FileChange> _queue;

        public QueueManager()
        {
            _queue = new ConcurrentQueue<FileChange>();
        }

        // Adicionar um arquivo à fila
        public void Enqueue(FileChange fileChange)
        {
            // Verifica se o arquivo já está na fila para evitar duplicidade
            if (!_queue.Any(fc => fc.FilePath == fileChange.FilePath))
            {
                _queue.Enqueue(fileChange);
                Console.WriteLine($"Arquivo adicionado à fila: {fileChange.FilePath}");
            }
        }

        // Remover o próximo arquivo da fila
        public bool TryDequeue(out FileChange? fileChange)
        {
            return _queue.TryDequeue(out fileChange);
        }

        // Verificar se a fila está vazia
        public bool IsEmpty()
        {
            // !_queue.Any();
            return !_queue.Any();
        }

        // Obter o número de itens na fila
        public int Count()
        {
            return _queue.Count;
        }

        // Exibir os itens na fila (para depuração)
        public void PrintQueue()
        {
            foreach (var item in _queue)
            {
                Console.WriteLine($"Na fila: {item.FilePath}");
            }
        }
    }
}
