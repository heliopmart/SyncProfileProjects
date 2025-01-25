import { initializeApp } from "firebase/app";
import { getFirestore } from "firebase/firestore";
import { getAnalytics } from "firebase/analytics"; 
import { getRemoteConfig } from "firebase/remote-config"; 
import { getStorage } from "firebase/storage";
import { getAuth } from "firebase/auth"; 

import { firebaseConfig } from '@/app/config/firebaseConfig';

const app = initializeApp(firebaseConfig);

const db = getFirestore(app); 
const analytics = getAnalytics(app); 
const remoteConfig = getRemoteConfig(app); 
const storage = getStorage(app); 
const auth = getAuth(app); 

export default db
export {analytics, remoteConfig, storage, auth}

console.log("Firebase configurado com sucesso!");
