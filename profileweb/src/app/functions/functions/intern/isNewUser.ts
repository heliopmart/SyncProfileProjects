export default function isNewUser(): boolean {
    const getCookie = (name: string) => {
        const value = `; ${document.cookie}`;
        const parts = value.split(`; ${name}=`);
        if (parts.length === 2) return parts.pop()?.split(';').shift();
        return null;
    };

    const newUser = getCookie('__user_agent');

    if (newUser) {
        return true;
    } else {
        const expirationDate = new Date();
        expirationDate.setDate(expirationDate.getDate() + 30); // 30 dias de validade
        document.cookie = `__user_agent=true; expires=${expirationDate.toUTCString()}; path=/`;
        return false;
    }
}
