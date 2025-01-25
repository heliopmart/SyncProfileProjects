'use client'
import { useEffect, useState, useRef } from 'react';
import Image from 'next/image'
import Link from 'next/link';
import "./style.scss"

const GitHubIcon = "/images/github_icon.svg"
const ShareIcon = "/images/share_icon.svg"

interface InformationProject {
    html_url: string;
    language: string;
    name: string;
    id: string;
    fullname: string;
    description: string;
}

export default function SoftwareProjects({ languageSelect }: { languageSelect: string }) {
    const [projects, setProjects] = useState<InformationProject[]>([]);
    const projectRefs = useRef<(HTMLDivElement | null)[]>([]); 


    function generateProjectLink(id: string) {
        return `/project/page/${id}`
    }

    function openProject(id: string) {
        const link = generateProjectLink(id);
        console.log(link) //TODO
        // Abrir projeto
    }

    function shareProjetc(id: string) {
        const link = generateProjectLink(id);
        console.log(link)
        //
    }

    // Função para aplicar a visibilidade
    const setVisible = (index: number) => {
        if (projectRefs.current[index]) {
            projectRefs.current[index]?.classList.add("visible");
        }
    };

    function FilterInformation(data:Array<InformationProject>|null) {
        if(data == null){
            return;
        }

        if(data.length > 0){
            const filtered = data.filter(key => key.id != "912198460").map((key: InformationProject) => {
                return {
                    "html_url": key.html_url,
                    "language": key.language,
                    "name": key.name,
                    "id": key.id,
                    "fullname": key.fullname,
                    "description": key.description
                }
            });
            setProjects(filtered);
        }else{
            console.error("GitHub Api 100% used")
        }
    }

    useEffect(() => {
        async function fetchProjects() {
            // TODO: Habilitar para fazer chamadas para o github
            const res = await fetch("/api/github/getRepos");
            const data = await res.json();

            console.log(data)
            // const data = {error: true};
            if(data.error){
                console.error("GitHub Api 100% used")
                return 
            }
            FilterInformation(data); // => data
        }
        fetchProjects();
    }, []);

    useEffect(() => {
        setTimeout(() => {
            const observer = new IntersectionObserver((entries) => {
                console.log(entries)
              entries.forEach((entry, index) => {
                if (entry.isIntersecting) {
                  setVisible(index);
                }
              });
            }, { threshold: 0.1 , rootMargin: '0px 0px 200px 0px' });
      
            projectRefs.current.forEach((project) => {
              if (project) observer.observe(project);
            });
      
            return () => observer.disconnect();
        }, 100);
    }, [projects]);

    return (
        <div className="SoftwareProjects">
            <div className="LineLeft" />
            <div className="content-projects"  ref={(el) => { projectRefs.current[0] = el }}>
                {projects.length > 0 ? (
                    <>
                        {projects.map((key, index) => (
                            <div
                                key={`project-software-${key.name}-${index}-${languageSelect}`}
                                className="project">
                                <div onClick={() => openProject(key.id)} className="content">
                                    <span className="text titleProject">{key.name}</span>
                                    <p className="text pProject">{key.description}</p>
                                </div>
                                <div className="content-tags">
                                    <Link href={key.html_url} target="_blank" rel="noopener noreferrer">
                                        <div className="tagProject github">
                                            <Image src={GitHubIcon} alt='GITHUB' width={50} height={50} />
                                        </div>
                                    </Link>
                                    <div onClick={() => shareProjetc(key.id)} className="tagProject">
                                        <Image src={ShareIcon} alt='Share' width={50} height={50} />
                                    </div>
                                    {key.language ? (<div className="tagProject text"><span>{key.language}</span></div>) : ""}
                                </div>
                            </div>
                        ))}
                    </>
                ) : ""}
            </div>
        </div>
    );
}
