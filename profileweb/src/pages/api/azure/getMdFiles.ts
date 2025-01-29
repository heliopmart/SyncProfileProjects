import { BlobServiceClient } from "@azure/storage-blob";
import { NextApiRequest, NextApiResponse } from "next";

// Configuração do Azure Blob Storage
function AuthStorage() {
    const AZURE_STORAGE_CONNECTION_STRING = process.env.AZURE_STORAGE_CONNECTION_STRING;

    if (!AZURE_STORAGE_CONNECTION_STRING) {
        return false;
    }

    try {
        const blobServiceClient = BlobServiceClient.fromConnectionString(AZURE_STORAGE_CONNECTION_STRING);
        const containerClient = blobServiceClient.getContainerClient("mdfilesproject");
        return containerClient;
    } catch (ex) {
        console.error(`Authenticação Azure Storage Fail: (${ex})`);
    }
}

async function getMarkdownFile(fileName: string) {
    const auth = AuthStorage();
    if (!auth) {
        return;
    }

    const blobClient = auth.getBlockBlobClient(`${fileName}.md`);

    try {
        // Faz o download do arquivo .md
        const downloadBlockBlobResponse = await blobClient.download(0);
        const downloadedData = await streamToText(downloadBlockBlobResponse.readableStreamBody);
        return downloadedData;
    } catch (error) {
        console.error("Erro ao baixar o arquivo", error);
        return null;
    }
}

// Função para converter o stream em texto (adaptada para Node.js)
async function streamToText(stream: NodeJS.ReadableStream | undefined): Promise<string | undefined> {
    if (!stream) {
        return
    }

    return new Promise((resolve, reject) => {
        let result = "";

        stream.on("data", (chunk) => {
            result += chunk;
        });

        stream.on("end", () => {
            resolve(result);
        });

        stream.on("error", (err) => {
            console.error("Erro no stream de dados:", err);
            reject(err);
        });
    });
}

interface Query {
    fileName?: string;
}

export default async function handler(req: NextApiRequest, res: NextApiResponse) {
    const { fileName } = req.query as Query; // Recebe o nome do arquivo (ID ou link)

    if (!fileName) {
        return res.status(400).json({ error: "Project name is required" });
    }

    // Tenta obter o conteúdo HTML convertido
    const data = await getMarkdownFile(fileName);
    if (data) {
        // Retorna o conteúdo HTML gerado
        res.status(200).json({ data });
    } else {
        // Em caso de erro ao buscar o arquivo
        res.status(500).json({ error: "Failed to fetch the Markdown file" });
    }
}
