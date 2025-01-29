import Image from 'next/image';
import Link from 'next/link';
import "./style.scss";

import InterfaceLanguageFileAndProject from '@/app/i18n/FileAndProject'
import ProjectData, {GithubData, Public_files} from "@/app/interfaces/ProjectData"

const FileFolderIcon = "/images/FileFolderIcon.svg";

interface InterfaceFileAndProject{
    language: InterfaceLanguageFileAndProject,
    type: 'software' | 'mechanic',
    project: ProjectData
}

export default function FileAndProject({language, type, project}:InterfaceFileAndProject){

    const setProjectFiles = () => {
        if(project && type){
            if(type == 'software'){
                return null
            }else{
                return project.mechanic?.metaDataProject?.public_files as Public_files[]
            }
        }

        return null
    }

    const setProject = () => {
        if(project && type){
            if(type == 'software'){
                return project.software?.gitHubData as GithubData
            }else{
                return null
            }
        }

        return null
    }

    return (
        <section id="fileAndProject">
            <div className="content-title">
                <Image src={FileFolderIcon} alt={language.title} width={100} height={100} />
                <h5 className="text title titleAboutProject">{language.title}</h5>
            </div>

            {type == 'mechanic'?(
                <section className='sectionFiles'>
                    <span className='text title sectionTitle'>{language.sectionFiles}</span>
                    <div className="content-files">
                        {setProjectFiles()!=null ? (
                            <>
                                {setProjectFiles()?.map((key, index) => (
                                    <div key={`${key.name}-${index}`} className="contentFile">
                                        <span className='text fileName'>{key.name}</span>
                                        <span className='text fileType'>{key.extention}</span>
                                        <Link href={key.share} target='_blank'>
                                            <button title={language.downlaodTitle} className='text buttonDownload' id={`id-button-${key.name}`}>
                                                {language.downlaodTitle}
                                            </button>
                                        </Link>
                                    </div>
                                ))}
                            </>
                        ):(
                            <span className='text warningText'>{language.warningText}</span>
                        )}

                    </div>
                </section>
            ):""}

            <section className='sectionFiles'>
                <span className='text title sectionTitle'>{language.sectionProject}</span>
                <div className="content-files">
                    {setProject() ? (
                        <div className="contentFile">
                            <span className='text fileName'>{setProject()?.name || "Projeto GITHUB"}</span>
                            <span className='text fileType'>GITHUB ZIP</span>
                            <Link href={setProject()?.download_url || ""} target='_blank'>
                                <button title={language.downlaodTitle}  className='text buttonDownload' id="id-download-git">
                                    {language.downlaodTitle}
                                </button>
                            </Link>
                        </div>                        
                    ):(
                        <span className='text warningText'>{language.warningText}</span>
                    )}
                </div>
            </section>
        </section>
    )
}