'use client'
import { initializeApp } from "firebase/app";
import { getFirestore } from "firebase/firestore";

import { firebaseConfig } from '@/app/config/firebaseConfig';

const app = initializeApp(firebaseConfig);
const db = getFirestore(app); 

export default db
export {app}

