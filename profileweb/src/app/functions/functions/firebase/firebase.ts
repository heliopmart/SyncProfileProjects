import {collection, getDocs, doc, getDoc} from "firebase/firestore";
import db from '@/app/api/firebase/Auth'

import {FirebaseMetadataDocument} from '@/app/interfaces/ProjectData'

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
                        url_readme: doc.data()?.url_readme || "",
                        metaDataProject: {
                            description: doc.data().metaDataProject?.description || "",
                            public_files: doc.data().metaDataProject?.public_files || "",
                            url_image: doc.data().metaDataProject?.url_image || ""
                        }
                    } as FirebaseMetadataDocument)
                })
                resolve(returnData)
            }catch(ex){
                reject(false)
                console.error(`Firebase : get(), Error ${ex}`)
            }
            
        })
    }

    async getById(id:string):Promise<FirebaseMetadataDocument | null>{
        if(id==null){
            return null
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
                        url_readme: docSnap.data().url_readme,
                        metaDataProject: {
                            description: docSnap.data().metaDataProject?.description || "",
                            public_files: docSnap.data().metaDataProject?.public_files || "",
                            url_image: docSnap.data().metaDataProject?.url_image || ""
                        }
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