import Image from 'next/image'
import InterfaceAboutMe from '@/app/i18n/AboutMe'
import "./style.scss";

const SvgImageInAboutMe1 = "/images/data_analysis.svg"
const SvgImageInAboutMe2 = "/images/ddos_protection.svg"
const SvgImageInAboutMe3 = "/images/system_settings.svg"
const SvgMaleAvatar = "/images/male_avatar.svg"

export default function AboutMe({language, siteExperience} : {language:InterfaceAboutMe, siteExperience:string}){
    return (
        <main id="main_AboutMe">
            <div className="div-titleAboutMe">
                <Image src={SvgMaleAvatar} alt='' width={60} height={60} className='image_AboutMe'/>
                <h2 className='text title titleAboutMe'>{language.AboutMe.title}</h2>
            </div>
            <div className="content_aboutMe">
                <div className="content_images_AboutMe">
                    <Image src={SvgImageInAboutMe1} alt='' className='images_in imageMe'  width={60} height={60} />
                    <Image src={SvgImageInAboutMe2} alt='' className='images_in imageObjective'  width={60} height={60} />
                    <Image src={SvgImageInAboutMe3} alt='' className='images_in imageConclusion'  width={60} height={60} />
                </div>
                <p className='text paragraphAboutMe' dangerouslySetInnerHTML={{ __html: language.AboutMe.content[siteExperience as keyof typeof language.AboutMe.content] }}/>
            </div>
        </main>
    )
}