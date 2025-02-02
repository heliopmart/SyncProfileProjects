interface returnInterface{
    token: string,
    expire: string
}

export default async function AuthBackend(apiKey: string, secretKey: string): Promise<returnInterface | null> {
    try {
        const response = await fetch("https://syncprofilewebbackend-production.up.railway.app/token/auth", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                key: apiKey,
                secret: secretKey,
                _TCD: "clientWeb"
            })
        });

        const data = await response.json();

        if (!response.ok || !data.status) {
            throw new Error(data.error || "Falha na autenticação");
        }

        return {token: data.token, expire: data.expire};
    } catch (error) {
        console.error("Erro na autenticação:", error);
        return null;
    }
}
