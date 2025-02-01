import { NextApiRequest, NextApiResponse } from 'next';
import axios from 'axios';
import { v4 as uuidv4 } from 'uuid';

const AZURE_TRANSLATOR_KEY = process.env.NEXT_PUBLIC_AZURE_API_KEY;  // A chave da API Azure
const AZURE_REGION = process.env.NEXT_PUBLIC_AZURE_REGION;  // A região do serviço Azure
const AZURE_TRANSLATOR_URL = 'https://api.cognitive.microsofttranslator.com/translate?api-version=3.0';  // Endpoint correto

interface TranslationRequest {
    text: string;
    from: string;
    to: string[];
}

export default async function handler(req: NextApiRequest, res: NextApiResponse) {
    if (req.method === 'POST') {
        const { text, from, to }: TranslationRequest = req.body;

        // Verificando parâmetros necessários
        if (!text || !from || !to || !Array.isArray(to)) {
            return res.status(400).json({ error: 'Texto, idioma de origem e idiomas de destino são necessários.' });
        }

        try {
            const response = await axios({
                method: 'POST',
                url: AZURE_TRANSLATOR_URL,
                headers: {
                    'Ocp-Apim-Subscription-Key': AZURE_TRANSLATOR_KEY,  // Chave da API
                    'Content-Type': 'application/json',
                    'Ocp-Apim-Subscription-Region': AZURE_REGION,
                    'X-ClientTraceId': uuidv4().toString(),  // Gerando UUID para rastreamento
                },
                params: {
                    from,
                    to: to.join(','),  // Converte o array de idiomas para string separada por vírgulas
                },
                data: [{
                    text: text,
                }],
            });


            // Retornando a tradução
            return res.status(200).json(response.data);
        } catch (error) {
            console.error('Erro na tradução:', error);
            return res.status(500).json({ error: 'Erro ao traduzir o texto.' });
        }
    } else {    
        return res.status(405).json({ error: 'Método não permitido.' });
    }
}
