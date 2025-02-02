'use client'
import Image from 'next/image';
import { useEffect, useState } from 'react';
import {useAuth} from '@/app/hooks/useAuth'
import {getCache, setCache} from '@/app/utils/mdCache'
import InterfaceAboutProject from '@/app/i18n/AboutProject';
import ProjectData from "@/app/interfaces/ProjectData"
import "./style.scss";

const FileInFolderIcon = "/images/FileInFolderIcon.svg";

interface AboutProjectProps {
    language: InterfaceAboutProject;
    project: ProjectData; // Caminho do arquivo Markdown
    type: 'software' | 'mechanic'
}

export default function AboutProject({ language, project, type }: AboutProjectProps) {
    const {login, setRefreshedToken} = useAuth()
    const [content, setContent] = useState<string>(language.defaultInformationProject);
    const [isLoading, setIsLoading] = useState<boolean>(true);

    async function fetchMarkdownContent() {
        const token = await login()
        try {
            setIsLoading(true);

            let markdownContent;
            if(type != 'mechanic'){
                const response_readmeGit = await fetch("https://syncprofilewebbackend-production.up.railway.app/github/md", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                        "Authorization": `Bearer ${token}`
                    },
                    body: JSON.stringify({
                        repoName: project.software?.gitHubData.name || ""
                    })
                });

                const data_readmeGit = await response_readmeGit.json();

                if(!data_readmeGit.status){
                    return 
                }

                if(data_readmeGit.refreshed){
                    setRefreshedToken(data_readmeGit.token)
                }

                markdownContent = data_readmeGit.data
            }else{
                if(getCache(project.mechanic?.url_readme || "")){
                    markdownContent = getCache(project.mechanic?.url_readme || "")
                }else{

                    const response_readmeAzure = await fetch("https://syncprofilewebbackend-production.up.railway.app/github/md", {
                        method: "POST",
                        headers: {
                            "Content-Type": "application/json",
                            "Authorization": `Bearer ${token}`
                        },
                        body: JSON.stringify({
                            repoName: project.mechanic?.url_readme || ""
                        })
                    });
    
                    const data_readmeAzure = await response_readmeAzure.json();
    
                    if(!data_readmeAzure.status){
                        return 
                    }
    
                    if(data_readmeAzure.refreshed){
                        setRefreshedToken(data_readmeAzure.token)
                    }

                    setCache(project.mechanic?.url_readme || "", data_readmeAzure.data)
                    markdownContent = data_readmeAzure.data
                }
            }

            const response = await fetch(`https://syncprofilewebbackend-production.up.railway.app/render/md`, {
                method: 'POST',
                headers: {
                  'Content-Type': 'application/json',
                  "Authorization": `Bearer ${token}`
                },
                body: JSON.stringify({ markdownContent })
            });
            const data = await response.json();

            if(!data.status){
                return 
            }

            if(data.refreshed){
                setRefreshedToken(data.token)
            }

            if (data.data) {
                setContent(data.data);
            } else {
                setContent(language.defaultInformationProject);
            }
        } catch (error) {
            console.error('Erro ao carregar o Markdown:', error);
            setContent(language.defaultInformationProject);
        } finally {
            setIsLoading(false);
        }
    }

    useEffect(() => {
        if (project && (project.software?.gitHubData.name || project.mechanic?.id)) {
            fetchMarkdownContent();
        }
    }, []);

    return (
        <main id="aboutProject">
            <div className="content-title">
                <Image src={FileInFolderIcon} alt={language.title} width={100} height={100} />
                <h5 className="text title titleAboutProject">{language.title}</h5>
            </div>
            <section className="section-aboutProject">
                {isLoading ? (
                    <p>Carregando conteúdo...</p>
                ) : (
                    <div
                        className="content_readme"
                        dangerouslySetInnerHTML={{ __html: content }}
                    />
                )}
            </section>
        </main>
    );
}
