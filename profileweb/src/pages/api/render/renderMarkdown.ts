import { remark } from 'remark';
import html from 'remark-html';
import type { NextApiRequest, NextApiResponse } from 'next';

export default async function handler(req: NextApiRequest, res: NextApiResponse): Promise<void> {
    try {
        if (req.method !== 'POST') {
            return res.status(405).json({ error: 'Method Not Allowed' });
        }

        const { markdownContent } = req.body;

        if (!markdownContent) {
            return res.status(400).json({ error: 'Markdown content is required' });
        }

        console.log(markdownContent)

        const convert = await remark().use(html).process(markdownContent);
        
        console.log(convert)

        // Converte o Markdown para HTML
        const htmlContent = convert

        // Retorna o conteúdo HTML gerado
        res.status(200).json({ content: htmlContent.toString() });
    } catch (error) {
        console.error('Erro ao processar o conteúdo Markdown:', error);
        res.status(500).json({ error: 'Failed to process Markdown content' });
    }
}
