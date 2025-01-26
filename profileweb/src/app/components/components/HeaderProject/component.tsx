import React, {useState} from 'react'
import Image from 'next/image'
import Link from 'next/link'
import UselessBlob from 'useless-blobs/lib/components';
import { TypeAnimation } from 'react-type-animation';
import "./style.scss"
import 'animate.css';

import HeaderInterface from '@/app/i18n/HeaderProject'

const ArrowLeftIcon = "/images/arrowLeftIcon.svg"

type OptionLanguageSelect = (res: React.ChangeEvent<HTMLSelectElement>) => void;
type LanguageSelect = (res: string) => void;

const NameProjectComponentAnimation = ({ Text }: { Text: string }) => {
    return (
      <TypeAnimation
        sequence={[Text, 1000]}
        wrapper="h3"
        cursor={false}
        key={Text}
        repeat={0}
        className={"text titleNameProject"}
      />
    );
};

export default function HeaderIndex({languageByIndex, languageSelect, language}:{languageByIndex:string, languageSelect:LanguageSelect, language:HeaderInterface}){
    const [flag, setFlag] = useState<string>(languageByIndex!=null ? languageByIndex : 'br');

    const OptionLanguageSelect:OptionLanguageSelect = (event: React.ChangeEvent<HTMLSelectElement>) => {
        const flag = event.target.value
        setFlag(flag)
        languageSelect(flag)
    }

    return (
        <header id="headerProject">
            <section className="sectionHead">
                <div id="div-button">
                    <Link href={"/"}>
                        <button className='text buttonBack'>
                            <Image src={ArrowLeftIcon} alt='' width={40} height={40}/>
                            {language.header.head.buttonTitle}
                        </button>
                    </Link>
                </div>
                <div id="div-language">
                    <label htmlFor="optionlanguage">
                        <div id="iconFlag" className={flag == "br" ? "flag-br" : "flag-us"}></div>
                    </label>
                    <select id="optionlanguage" value={languageByIndex} onChange={OptionLanguageSelect} title={language.header.head.selectTittle} className='spantext optionlanguage'>
                        <option value={'br'}>Português</option>
                        <option value={'us'}>English</option>
                    </select>
                </div>
            </section>
            <section className='sectionProjectName'>
                <div className="div-titileProject">
                    <NameProjectComponentAnimation key={"Text-Version"+flag} Text={language.header.head.defaultTitle}/>
                </div>
                <div className="div-subtextProject">
                    <h1 className='text pProjectSubText' dangerouslySetInnerHTML={{ __html: language.header.head.defaultSubText}}/>
                </div>
            </section>
            <section className='sectionCuriosity'>
               <div className="content-blob-curiosity animate__animated animate__fadeInUp">
                    <UselessBlob
                        fill='#556EA7'
                        stroke='none'
                        verts={3}
                        height={310}
                        width={400}
                        irregularity={0}
                        spikiness={0.2}
                        boundingShape='rectangle'
                        className='blobStyle'
                    />
                    <div className="languages-style content-information-blob">
                        <span className='text languageStyleText infoText'>C#</span>
                        <span className='text languageStyleText infoText'>NEXT</span>
                        <span className='text languageStyleText infoText'>TypeScript</span>
                        <span className='text languageStyleText infoText'>Firebase</span>
                        <span className='text languageStyleText infoText'>Azure</span>
                    </div>
               </div>
               <div className="content-blob-curiosity animate__animated animate__fadeInUp">
                    <UselessBlob
                        fill='#556EA7'
                        stroke='none'
                        verts={3}
                        height={310}
                        width={400}
                        irregularity={0}
                        spikiness={0.2}
                        boundingShape='rectangle'
                        className='blobStyle'
                    />
                    <div className="languages-style content-information-blob">
                        {/* TODO: Tipo de projeto */}
                        <span className='text titleBlob infoText'>{language.header.curiosity.typeProject[0]}</span>
                    </div>
               </div>
               <div className="content-blob-curiosity animate__animated animate__fadeInUp">
                    <UselessBlob
                        fill='#556EA7'
                        stroke='none'
                        verts={3}
                        height={310}
                        width={400}
                        irregularity={0}
                        spikiness={0.2}
                        boundingShape='rectangle'
                        className='blobStyle'
                    />
                    <div className="languages-style content-information-blob">
                        <span className='text titleBlob infoText'>
                            {/* TODO: adicionar tempo de projeto */}
                            {language.header.curiosity.createDefaultString[0]} 2 {language.header.curiosity.createDefaultString[2]}
                        </span>
                    </div>
               </div>
            </section>
        </header>
    );
}

