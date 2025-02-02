'use client'
import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import {useAuth} from '@/app/hooks/useAuth'
import AOS from 'aos';
import 'aos/dist/aos.css';
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
    const {login, setRefreshedToken} = useAuth()
    const [projects, setProjects] = useState<InformationProject[]>([]);
    const router = useRouter();
    
    function generateProjectLink(id: string) {
        return `/project/${languageSelect}/software/${id}`
    }
    
    function openProject(id: string) {
        const link = generateProjectLink(id);
        console.log(link)
        router.push(link);
    }

    function shareProjetc(id: string) {
        const link = generateProjectLink(id);
        // TODO copy link
        console.log(link)
    }


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
            const token = await login()

            const res = await fetch("https://syncprofilewebbackend-production.up.railway.app/github/repo", {
                method: "GET",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}`
                }
            });
            const data = await res.json();

            if(data.refreshed){
                setRefreshedToken(data.token)
            }

            if(!data.status){
                console.error("GitHub Api 100% used")
                return 
            }
            FilterInformation(data.data);
        }
        fetchProjects();
    }, []);

    useEffect(() => {
        AOS.init();
    }, []);
    

    return (
        <div className="SoftwareProjects">
            <div className="LineLeft" />
            <div className="content-projects" >
                {projects.length > 0 ? (
                    <>
                        {projects.map((key, index) => (
                            <div
                                key={`project-software-${key.name}-${index}-${languageSelect}`}
                                className="project"  data-aos="fade-up">
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
