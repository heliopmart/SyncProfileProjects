import type { NextApiRequest, NextApiResponse } from 'next';
import config from './config.json'
async function handler(req: NextApiRequest, res: NextApiResponse) {
    const { url } = req.query;
    if (!config.user || !url) {
        return res.status(400).json({ error: 'Missing owner or repo_id in query parameters.' });
    }
    try {
        const repoResponse = await fetch(`${url}`);

        if (!repoResponse.ok) {
            return res.status(repoResponse.status).json({ error: 'Failed to fetch repository data.' });
        }

        const repoData = await repoResponse.json();

        if (repoData) {
            return res.status(200).json(repoData);
        } else {
            return res.status(404).json({ error: 'languages not found.' });
        }
    } catch (ex) {
        console.error('Error get languages repos by id:', ex);
        return res.status(500).json({ error: 'Internal server error' });
    }
}

export default handler;