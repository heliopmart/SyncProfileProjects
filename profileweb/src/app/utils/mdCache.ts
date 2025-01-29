interface CacheItem {
  value: unknown;
  expiresAt: number;
}

const cache: { [key: string]: CacheItem } = {};

function verifyCache(key: string | number, value : unknown){
    if(getCache(key)){
      return getCache(key)
    }else{
      setCache(key, value)
      return value
    }
}

function setCache(key: string | number, value: unknown, ttl = 3600) {
  const now = Date.now();
  cache[key] = {
    value,
    expiresAt: now + ttl * 1000,  
  };
}

function getCache(key: string | number) {
  const now = Date.now();
  const cachedItem = cache[key];

  if (cachedItem && cachedItem.expiresAt > now) {
    return cachedItem.value;  
  }

  delete cache[key];  
  return null;
}

export { setCache, getCache, verifyCache};
