import {collection, getDocs, doc, getDoc} from "firebase/firestore";
import db, {analytics, auth, remoteConfig, storage} from '@/app/api/firebase/Auth'

export interface FirebaseMetadataDocument{
    AsyncTime: string,
    DateTime: string,
    Device: string,
    FolderId: string,
    Id: string,
    Name: string,
    Status: number
    Image: string | null
}

export default class Firebase{
    constructor(){
    }

    async get() : Promise<FirebaseMetadataDocument[]>{
        return new Promise( async (resolve, reject) => {
            try{
                const querySnapshot = await getDocs(collection(db, "metadata"));
                const returnData:FirebaseMetadataDocument[] = []
                querySnapshot.forEach((doc) => {
                    returnData.push({
                        Id: doc.id,
                        AsyncTime: doc.data().AsyncTime,
                        DateTime: doc.data().DateTime,
                        Device: doc.data().Device,
                        FolderId: doc.data().FolderId,
                        Name: doc.data().Name,
                        Status: doc.data().Status,
                        Image: null
                    })
                })
                resolve(returnData)
            }catch(ex){
                reject(false)
                console.error(`Firebase : get(), Error ${ex}`)
            }
            
        })
    }

    async getById(id:string){
        if(id!=null){
            return false
        }

        return new Promise( async (resolve, reject) => {
            try{
                const docRef = doc(db, "metadata", id);
                const docSnap = await getDoc(docRef);

                if (docSnap.exists()) {
                    const returnData:FirebaseMetadataDocument = {
                        Id: id,
                        AsyncTime: docSnap.data().AsyncTime,
                        DateTime: docSnap.data().DateTime,
                        Device: docSnap.data().Device,
                        FolderId: docSnap.data().FolderId,
                        Name: docSnap.data().Name,
                        Status: docSnap.data().Status,
                        Image: null
                    }
                    resolve(returnData)
                } else {
                    reject(false)
                    console.error(`Firebase : getById(), Error: Documento não encontrado por ID`)

                }
            }catch(ex){
                reject(false)
                console.error(`Firebase : getById(), Error: ${ex}`)
            }
            
        })
    }
}