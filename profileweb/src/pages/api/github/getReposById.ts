import type { NextApiRequest, NextApiResponse } from 'next';
import config from './config.json'
async function handler(req: NextApiRequest, res: NextApiResponse) {
    const { repo_id } = req.query;
    if (!config.user || !repo_id) {
        return res.status(400).json({ error: 'Missing owner or repo_id in query parameters.' });
    }
    try {
        const repoResponse = await fetch(`https://api.github.com/repositories/${repo_id}`);

        if (!repoResponse.ok) {
            return res.status(repoResponse.status).json({ error: 'Failed to fetch repository data.' });
        }

        const repoData = await repoResponse.json();

        if (repoData) {
            return res.status(200).json(repoData);
        } else {
            return res.status(404).json({ error: 'README.md not found.' });
        }
    } catch (ex) {
        console.error('Error get repos by id:', ex);
        return res.status(500).json({ error: 'Internal server error' });
    }
}

export default handler;