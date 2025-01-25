import axios from 'axios';

const handleTranslate = async (text: string, targetLanguage: string):Promise<string> => {
    try {
        const response = await axios.post('/api/azure/translate', {
            text: text,
            from: 'pt',  // Idioma de origem (pode ser configurado dinamicamente)
            to: [targetLanguage == 'br' ? 'pt' : 'en'],  // Idiomas de destino, exemplo 'en' ou 'pt'
        });

        return response.data[0].translations[0].text
    } catch (error) {
        console.error('Erro ao traduzir:', error);
        return text;
    }
};

export default handleTranslate;