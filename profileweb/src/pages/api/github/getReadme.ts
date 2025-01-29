import type { NextApiRequest, NextApiResponse } from 'next';
import config from './config.json'
async function handler(req: NextApiRequest, res: NextApiResponse) {
  const {repoName } = req.query;
  if (!config.user || !repoName) {
    return res.status(400).json({ error: 'Missing owner or repoName in query parameters.' });
  }

  try {
    const readmeResponse = await fetch(`https://api.github.com/repos/${config.user}/${repoName}/contents/README.md`);
    
    if (!readmeResponse.ok) {
      return res.status(readmeResponse.status).json({ error: 'Failed to fetch README.md.' });
    }

    const readmeData = await readmeResponse.json();

    // Verifica se o arquivo README.md existe e decodifica o conteúdo base64
    if (readmeData.content) {
      const decodedContent = Buffer.from(readmeData.content, 'base64').toString('utf-8');
      return res.status(200).json({ content: decodedContent });
    } else {
      return res.status(404).json({ error: 'README.md not found.' });
    }
  } catch (error) {
    console.error('Error fetching README.md:', error);
    return res.status(500).json({ error: 'Internal server error' });
  }
}

export default handler;
