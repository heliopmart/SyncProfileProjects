import { initializeApp } from "firebase/app";
import { getFirestore } from "firebase/firestore";
import { getAnalytics } from "firebase/analytics"; 
import { getRemoteConfig } from "firebase/remote-config"; 

import { firebaseConfig } from '@/app/config/firebaseConfig';

const app = initializeApp(firebaseConfig);

const db = getFirestore(app); 
const analytics = getAnalytics(app); 
const remoteConfig = getRemoteConfig(app); 

export default db
export {analytics, remoteConfig}

