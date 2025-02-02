const handleTranslate = async (token:string|null, text: string, targetLanguage: string):Promise<string> => {
    try{
        const res = await fetch("https://syncprofilewebbackend-production.up.railway.app/azure/translate", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${token}`
            },
            body: JSON.stringify({
                text: text,
                from: "pt",
                to: [targetLanguage == 'br' ? 'pt' : 'en'] 
            })
        });
        const data = await res.json();

        if(!data.status){
            console.error("Error: ", data.message)
            return text
        }

        return data.data[0].translations[0].text
    }catch(error){
        console.error("Error: ", error)
        return text
    }
}


export default handleTranslate;

