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

/*
  Principais informações do res()

  html_url => link para o projeto
  language => principal linguagem utilizada
  name => nome do projeto
  id => id
  fullname => {profile}{name}

  existe a possibilidade de pegar todas as tecnologias utilizadas no projeto
*/