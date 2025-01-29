import config from './config.json'
import { NextApiRequest, NextApiResponse } from 'next';

export default async function handler(req: NextApiRequest, res: NextApiResponse) {
  const response = await fetch(`${config.defaultUrl}/${config.user}/${config.request}`);

  if (response.ok) {
    const projects = await response.json();
    res.status(200).json(projects);
  } else {
    res.status(response.status).json({ error: 'Erro ao buscar repositórios' });
  }
}
