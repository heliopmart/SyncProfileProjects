import { useState } from "react";
import {backendConfig} from '@/app/config/backendConfig'
import AuthBackend from "@/app/api/auth/auth.backend";

export function useAuth() {
    const [loading, setLoading] = useState<boolean>(false);
    const [error, setError] = useState<string | null>(null);

    async function login(): Promise<string | null> {
        setLoading(true);
        setError(null);
    
        const authExpire = localStorage.getItem("authExpire");
        if (localStorage.getItem("authToken") && authExpire && !__isExpireWithinOneHour(authExpire)) {
            setLoading(false);
            return localStorage.getItem("authToken") ; // 🔹 Retorna o token atual se ainda for válido
        }
    
        const authToken = await AuthBackend(backendConfig.apiKey, backendConfig.secretKey);
    
        if (authToken) {
            localStorage.setItem("authToken", authToken.token);
            localStorage.setItem("authExpire", authToken.expire);
            setLoading(false);
            return authToken.token;
        } else {
            setError("Credenciais inválidas");
            setLoading(false);
            return null;
        }
    }

    async function setRefreshedToken(token:string){
        localStorage.setItem("authToken", token);
        localStorage.setItem("authExpire", new Date().getTime().toString());
    }

    function logout() {
        localStorage.removeItem("authToken");
    }

    return { setRefreshedToken, login, logout, loading, error };
}

function __isExpireWithinOneHour(expireTimestamp:string) {
    const expireDate = new Date(expireTimestamp);
    const now = new Date();

    const diffInMinutes = (expireDate.getTime() - now.getTime()) / (1000 * 60);

    return diffInMinutes > 0 && diffInMinutes <= 60;
}
