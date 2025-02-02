import Image from 'next/image'
import Link from 'next/link'
import { translateWithCache } from '@/app/utils/cache';
import { useEffect, useState } from 'react'
import {Translate, Firebase} from '@/app/functions/functions'
import {FirebaseMetadataDocument} from '@/app/interfaces/ProjectData'
import {useAuth} from '@/app/hooks/useAuth'
import "./style.scss"

const BackgrundImage1 = "/images/Default_background_1.webp"
const BackgrundImage2 = "/images/Default_background_2.webp"

const selectRandomImage = () => {
    return Math.floor(Math.random()* 2) == 0 ? BackgrundImage1 : BackgrundImage2
}

export default function EnginnerProjects({languageSelect}:{languageSelect:string}){
    const {login} = useAuth()
    const [projects, setProjects] = useState<FirebaseMetadataDocument[]>([])

    async function fetchProjects() {
        const token = await login()

        const fetchedProjects = await new Firebase().get();
        const translatedProjects = await Promise.all(
          fetchedProjects.map(async (project) => ({
            ...project,
            Name: languageSelect == 'br' ? project.Name : await translateWithCache(token, project.Name, languageSelect == 'br' ? 'pt': 'en', Translate),
            Description: languageSelect == 'br' ?  project?.metaDataProject?.description || null : await translateWithCache(token, project?.metaDataProject?.description || null, languageSelect == 'br' ? 'pt': 'en', Translate),
          }))
        );
        setProjects(translatedProjects as FirebaseMetadataDocument[]);
    }
    
    useEffect(() => {
        fetchProjects();
    },[])

    useEffect(() => {
        fetchProjects()
    },[languageSelect])
    
    return (
        <div className="EnginnerProjects">
            <div className="content-projects">
                {projects && projects.length > 0 && projects[0].Name != null ? (
                    <>
                        {projects.map((key, index) => (
                            <Link href={`project/${languageSelect}/mechanic/${key.Id}`} key={`${key.Name}-${index}-${languageSelect}`}>
                                <div className="projects">
                                    <div className="imageProject">
                                        <Image src={key.metaDataProject?.url_image || selectRandomImage()} alt={key.Name} width={300} height={200}/>
                                    </div>
                                    <span className='text nameProject'>{key.Description || key.Name}</span>
                                </div>
                            </Link>
                        ))}
                    </>
                ): ""}
            </div>
        </div>
    )
}