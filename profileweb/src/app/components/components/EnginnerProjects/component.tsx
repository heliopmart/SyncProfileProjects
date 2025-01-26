import Image from 'next/image'
import { translateWithCache } from '@/app/utils/cache';
import { useEffect, useState } from 'react'
import {Translate, Firebase, FirebaseMetadataDocument} from '@/app/functions/functions'
import "./style.scss"

// const WithoutImageIcon = "/images/warning_icon.svg"

export default function EnginnerProjects({languageSelect}:{languageSelect:string}){
    const [projects, setProjects] = useState<FirebaseMetadataDocument[]>([])

    async function fetchProjects() {
        const fetchedProjects = await new Firebase().get();
        const translatedProjects = await Promise.all(
          fetchedProjects.map(async (project) => ({
            ...project,
            Name: languageSelect == 'br' ? project.Name : await translateWithCache(project.Name, languageSelect == 'br' ? 'pt': 'en', Translate),
            // TODO Description: await translateWithCache(project.Description, languageSelect, translateText),
          }))
        );
        setProjects(translatedProjects);
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
                            <div key={`${key.Name}-${index}-${languageSelect}`} className="projects">
                                <div className="imageProject">
                                    {key.Image ? (
                                        <Image src={key.Image} alt={key.Name} width={300} height={200}/>
                                    ):(
                                        <div className='text withoutImage'/>
                                    )}
                                </div>
                                <span className='text nameProject'>{key.Name}</span>
                            </div>
                        ))}
                    </>
                ): ""}
            </div>
        </div>
    )
}