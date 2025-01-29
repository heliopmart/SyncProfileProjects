import React, { useState } from 'react'
import Image from 'next/image'
import Link from 'next/link'
import UselessBlob from 'useless-blobs/lib/components';
import { TypeAnimation } from 'react-type-animation';
import "./style.scss"
import 'animate.css';

import HeaderInterface from '@/app/i18n/HeaderProject'
import ProjectData from "@/app/interfaces/ProjectData"

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

export default function HeaderIndex({isMobile=false, type, languageByIndex, languageSelect, language, project }: { languageByIndex: string, languageSelect: LanguageSelect, language: HeaderInterface, project?: ProjectData, type: 'software' | 'mechanic', isMobile: boolean}) {
    const [flag, setFlag] = useState<string>(languageByIndex != null ? languageByIndex : 'br');

    const OptionLanguageSelect: OptionLanguageSelect = (event: React.ChangeEvent<HTMLSelectElement>) => {
        const flag = event.target.value
        setFlag(flag)
        languageSelect(flag)
    }

    const titlePage = (): string => {
        if (project && type && (project.mechanic || project.software)) {
            if (type == 'software') {
                return project.software?.gitHubData.name || language.header.head.defaultTitle
            } else {
                return project.mechanic?.name || language.header.head.defaultTitle
            }
        } else {
            return language.header.head.defaultTitle
        }
    }

    const languagesUse = (): string[] | null => {
        if (project && type) {
            if (type == 'software') {
                return project.software?.languages || null
            } else {
                return []
            }
        } else {
            return null
        }
    }

    const typeProject = (): string | null => {
        if (project && type) {
            if (type == 'software') {
                return language.header.curiosity.typeProject[0]
            } else {
                return language.header.curiosity.typeProject[1]
            }
        } else {
            return null
        }
    }

    const creationTime = () : string => {
        if (project && type) {
            if (type == 'software') {
                const createTime = (project.software?.gitHubData.creationTime ?? 0) < 0 ? ">1" : project.software?.gitHubData.creationTime ?? 0
                return `${language.header.curiosity.createDefaultString[0]} ${createTime} ${language.header.curiosity.createDefaultString[project.software?.gitHubData.creationTime || 1 > 1 ? 2 : 1]}`
            } else {
                const createTime = (project.mechanic?.creationTime ?? 0) < 0 ? ">1" : project.mechanic?.creationTime ?? 0                
                return `${language.header.curiosity.createDefaultString[0]} ${createTime} ${language.header.curiosity.createDefaultString[project.mechanic?.creationTime || 1 > 1 ? 2 : 1]}`
            }
        } else {
            return "Em criação"
        }
    }

    return (
        <header id="headerProject">
            <section className="sectionHead">
                <div id="div-button">
                    <Link href={"/"}>
                        <button className='text buttonBack'>
                            <Image src={ArrowLeftIcon} alt='' width={40} height={40} />
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
                    <NameProjectComponentAnimation key={"Text-Version" + flag} Text={titlePage()} />
                </div>
                <div className="div-subtextProject">
                    <h1 className='text pProjectSubText' dangerouslySetInnerHTML={{ __html: language.header.head.defaultSubText }} />
                </div>
            </section>
            <section className='sectionCuriosity'>
                {languagesUse() && languagesUse()!.length > 1 ? (
                    <div className="content-blob-curiosity animate__animated animate__fadeInUp">
                        <UselessBlob
                            fill='#556EA7'
                            stroke='none'
                            verts={3}
                            height={isMobile ? 280 : 310}
                            width={isMobile ? 300 : 400}
                            irregularity={0}
                            spikiness={0.2}
                            boundingShape='rectangle'
                            className='blobStyle'
                        />
                        <div className="languages-style content-information-blob">
                            {languagesUse()?.map((key, index) => (
                                <span key={`${key}-${index}`} className='text languageStyleText infoText'>{key}</span>
                            ))}
                        </div>
                    </div>
                ) : ""}

                <div className="content-blob-curiosity animate__animated animate__fadeInUp">
                    <UselessBlob
                        fill='#556EA7'
                        stroke='none'
                        verts={3}
                        height={isMobile ? 280 : 310}
                        width={isMobile ? 300 : 400}
                        irregularity={0}
                        spikiness={0.2}
                        boundingShape='rectangle'
                        className='blobStyle'
                    />
                    <div className="languages-style content-information-blob">
                        <span className='text titleBlob infoText'>{typeProject()}</span>
                    </div>
                </div>
                <div className="content-blob-curiosity animate__animated animate__fadeInUp">
                    <UselessBlob
                        fill='#556EA7'
                        stroke='none'
                        verts={3}
                        height={isMobile ? 280 : 310}
                        width={isMobile ? 300 : 400}
                        irregularity={0}
                        spikiness={0.2}
                        boundingShape='rectangle'
                        className='blobStyle'
                    />
                    <div className="languages-style content-information-blob">
                        <span className='text titleBlob infoText'>
                            {creationTime()}
                        </span>
                    </div>
                </div>
            </section>
        </header>
    );
}

