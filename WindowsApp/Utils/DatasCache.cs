using System.Collections.Concurrent;

public sealed class CentralCache
{
    private static readonly Lazy<CentralCache> _instance = new(() => new CentralCache());
    private ConcurrentDictionary<string, object> _cache;

    // Construtor privado (somente acessível dentro da classe)
    private CentralCache()
    {
        _cache = new ConcurrentDictionary<string, object>();
    }

    // Acesso à instância única
    public static CentralCache Instance => _instance.Value;

    // Adicionar um valor ao cache
    public void AddToCache(string key, object value)
    {
        _cache[key] = value;
    }

    // Obter um valor do cache
    public object? GetFromCache(string key)
    {
        return _cache.TryGetValue(key, out var value) ? value : null;
    }

    // use => var userName = CentralCache.Instance.GetFromCache("UserName");

    // Verificar se a chave existe no cache
    public bool ContainsKey(string key)
    {
        return _cache.ContainsKey(key);
    }

    // Limpar todo o cache
    public void ClearCache()
    {
        _cache.Clear();
    }
}
