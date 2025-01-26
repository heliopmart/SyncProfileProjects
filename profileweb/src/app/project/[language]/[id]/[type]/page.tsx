'use client'
import {useState} from 'react'
import { useParams } from 'next/navigation'; 
import './style.scss';

// import component
import {HeaderProject, SectionHeaderProjectImage, AboutProject} from '@/app/components/components'

// import languages interface

// import languages json

import HeaderProjectBr from '@/app/i18n/HeaderProject_br.json'
import HeaderProjectUs from '@/app/i18n/HeaderProject_us.json'

import AboutProjectBr from '@/app/i18n/AboutProject_br.json'
import AboutProjectUs from '@/app/i18n/AboutProject_us.json'


export default function Project() {
    const [language, setLanguage] = useState<string>('br');
    const params = useParams();
    
    const getLanguage = params?.language as string;
    const getType = params?.type as string;
    const getId = params?.id as string;


    const getSelectedLanguage = (res:string) => {
      setLanguage(res)
    }

    return (
        <div className="ProjectPage">
            <HeaderProject language={language == "br" ? HeaderProjectBr : HeaderProjectUs} languageSelect={getSelectedLanguage} languageByIndex={language}/>
            <SectionHeaderProjectImage/>
            <AboutProject language={language == 'br' ? AboutProjectBr : AboutProjectUs} project={{}}/>
            <span>{getLanguage}</span>
            <span>{language}</span>
            <span>{getType}</span>
            <span>{getId}</span>
        </div>
    );
}
