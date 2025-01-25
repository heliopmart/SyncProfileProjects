// Definindo o cache como um objeto simples em memória
interface Cache {
  [key: string]: string;  // Armazenamento das traduções por texto e idioma
}

// Cache simples em memória
const cache: Cache = {};

// Função para verificar se a tradução já existe no cache
export const getFromCache = (key: string): string | undefined => {
  return cache[key];
};

// Função para armazenar no cache
export const addToCache = (key: string, value: string): void => {
  cache[key] = value;
};

// Função para criar a chave do cache
const generateCacheKey = (text: string, targetLanguage: string): string => {
  return `${text}_${targetLanguage}`;  // A chave é única para o texto e o idioma
};

// Função para tradução com cache
export const translateWithCache = async (text: string, targetLanguage: string, translateFn: (text: string, targetLanguage: string) => Promise<string>) => {
  // Se o idioma de destino for português, retorna o texto original sem tradução
  if (targetLanguage === 'pt') {
    return text;
  }

  // Gera a chave única do cache com base no texto e no idioma
  const cacheKey = generateCacheKey(text, targetLanguage);

  // Verifica se a tradução já está no cache
  const cachedTranslation = getFromCache(cacheKey);

  // Se encontrar no cache, retorna a tradução já armazenada
  if (cachedTranslation) {
    return cachedTranslation;
  }

  // Caso contrário, realiza a tradução e armazena no cache
  const translatedText = await translateFn(text, targetLanguage);
  addToCache(cacheKey, translatedText);  // Armazena no cache
  return translatedText;
};
