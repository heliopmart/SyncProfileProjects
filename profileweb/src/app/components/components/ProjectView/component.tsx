import Image from 'next/image'
import {SoftwareProjects, EnginnerProjects} from '../../componets'
import InterfaceProjectView  from '@/app/i18n/ProjectView'
import "./style.scss"

const FileInFolderIcon = "/images/FileInFolderIcon.svg";
const FileFolderIcon = "/images/FileFolderIcon.svg";


export default function ProjectView({language, type, languageSelect}:{language:InterfaceProjectView, type:'software'|'enginner', languageSelect:string}){
    return (
        <section className="ProjectView">
            <div className="content-title-section">
                <Image src={type=='software'? FileFolderIcon : FileInFolderIcon} alt='' className="image-section" width={100} height={100}/>
                <h5 className='text title ProjectViewTitle'>{language[type].title}</h5>
            </div>
            <div className="content-projects">
                {type=='software' ? (
                    <SoftwareProjects languageSelect={languageSelect}/>
                ):(
                    <EnginnerProjects languageSelect={languageSelect}/>
                )}
            </div>
        </section>
    )
}