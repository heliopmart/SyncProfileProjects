'use client'
import Image from 'next/image';
import { useEffect, useState } from 'react';
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
    const [content, setContent] = useState<string>(language.defaultInformationProject);
    const [isLoading, setIsLoading] = useState<boolean>(true);

    async function fetchMarkdownContent() {
        try {
            setIsLoading(true);

            let markdownContent;
            if(type != 'mechanic'){
                const response_readmeGit = await fetch(`/api/github/getReadme?repoName=${project.software?.gitHubData.name}`);
                const data_readmeGit = await response_readmeGit.json();
                markdownContent = data_readmeGit.content
            }else{
                console.log(getCache(project.mechanic?.url_readme || ""))
                if(getCache(project.mechanic?.url_readme || "")){
                    markdownContent = getCache(project.mechanic?.url_readme || "")
                }else{
                    const response_readmeAzure = await fetch(`/api/azure/getMdFiles?fileName=${project.mechanic?.url_readme || ""}`);
                    const data_readmeAzure = (await response_readmeAzure.json()).data;
                    setCache(project.mechanic?.url_readme || "", data_readmeAzure)
                    markdownContent = data_readmeAzure
                }
            }

            const response = await fetch(`/api/render/renderMarkdown`, {
                method: 'POST',
                headers: {
                  'Content-Type': 'application/json',
                },
                body: JSON.stringify({ markdownContent })
            });
            const data = await response.json();
            if (data.content) {
                setContent(data.content);
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
