import Image from 'next/image'
import "./style.scss"

import InterfaceAboutProject from '@/app/i18n/AboutProject'

const FileInFolderIcon = "/images/FileInFolderIcon.svg";

export default function AboutProject({language, project}:{language:InterfaceAboutProject, project:object}){
    return (
        <main id="aboutProject">
            <div className="content-title">
                <Image src={FileInFolderIcon} alt={language.title} width={100} height={100}/>
                <h5 className='text title titleAboutProject'>{language.title}</h5>
            </div>
            <section className='section-aboutProject'>
                <div className="content_readme">

                </div>
            </section>
        </main>
    )
}