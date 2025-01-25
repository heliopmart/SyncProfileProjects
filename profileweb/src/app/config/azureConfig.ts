export const azureConfig = {
    apiKey: process.env.NEXT_PUBLIC_AZURE_API_KEY || '',  // Obtenha a chave de API da variável de ambiente
    region: process.env.NEXT_PUBLIC_AZURE_REGION || '',  // Obtenha a região da variável de ambiente
};