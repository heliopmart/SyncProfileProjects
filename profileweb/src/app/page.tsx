'use client'

import { useState } from 'react';

// component

import { HeaderIndex, SectionHeaderImage, AboutMe, SectionAboutMeImage, SomeSkills, ProjectView, SectionProjectViewImage, Footer } from './components/components';

// Interfaces

import InterfaceAboutMe from '@/app/i18n/AboutMe'
import InterfaceSomeSkills from '@/app/i18n/SomeSkills'
import InterfaceProjectView from '@/app/i18n/ProjectView'
import InterfaceFooter from "@/app/i18n/Footer"

// languages

import LanguagesAboutMe_br from '@/app/i18n/AboutMe_br.json'
import LanguagesAboutMe_us from '@/app/i18n/AboutMe_us.json'
import LanguagesSomeSkills_br from '@/app/i18n/Someskills_br.json'
import LanguagesSomeSkills_us from '@/app/i18n/Someskills_us.json'
import LanguagesProjectView_br from '@/app/i18n/ProjectView_br.json'
import LanguagesProjectView__us from '@/app/i18n/ProjectView_us.json'
import LanguagesFooter_br from '@/app/i18n/Footer_br.json'
import LanguagesFooter__us from '@/app/i18n/Footer_us.json'


const HomePage: React.FC = () => {
    const [language, setLanguage] = useState<string>('br');
    const [siteExperience, setSiteExperience] = useState<string>('story');

    const getLanguage = (res: string) => {
        setLanguage(res)
    }
    const getSiteExperience = (res: React.MouseEvent<HTMLButtonElement>) => {
        setSiteExperience(res.currentTarget.id)
    }

    const isMobile = (): boolean => {
        const userAgent = navigator.userAgent.toLowerCase();
        if (window.matchMedia("(max-width: 1400px)").matches || /iphone|ipod|android|blackberry|windows phone|mobile|opera mini/.test(userAgent)) {
            return true;
        } else {
            return false;
        }
    }

    return (
        <div className="homePage">
            <HeaderIndex isMobile={isMobile()}  languageSelect={getLanguage} optionSelect={getSiteExperience} />
            <SectionHeaderImage />
            <AboutMe language={language == 'br' ? LanguagesAboutMe_br as InterfaceAboutMe : LanguagesAboutMe_us as InterfaceAboutMe} siteExperience={siteExperience} key={`AboutMe-${siteExperience}-${language}`} />
            <SectionAboutMeImage />
            <SomeSkills language={language == 'br' ? LanguagesSomeSkills_br as InterfaceSomeSkills : LanguagesSomeSkills_us as InterfaceSomeSkills} key={`SomeSkills-${siteExperience}-${language}`} />
            <ProjectView type='software' languageSelect={language} language={language == 'br' ? LanguagesProjectView_br as InterfaceProjectView : LanguagesProjectView__us as InterfaceProjectView} />
            <SectionProjectViewImage />
            <ProjectView type='enginner' key={"enginner-Project_view" + language} languageSelect={language} language={language == 'br' ? LanguagesProjectView_br as InterfaceProjectView : LanguagesProjectView__us as InterfaceProjectView} />
            <Footer isMobile={isMobile()} language={language == 'br' ? LanguagesFooter_br as InterfaceFooter : LanguagesFooter__us as InterfaceFooter} key={'footer' + language} />
        </div>
    );
};

export default HomePage;