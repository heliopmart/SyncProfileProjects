import {useState} from 'react'
import InterfaceSomeSkills  from '@/app/i18n/SomeSkills'
import {Analytics} from '@/app/functions/functions'
import Skills from '../SkillsComponent/component'
import "./style.scss"

export default function SomeSkills({language} : {language:InterfaceSomeSkills}){
    const [count, setCount] = useState<number>(0);

    function updateLineSoftware(btn:React.MouseEvent<HTMLButtonElement>){
        const typeById = btn.currentTarget.id
        if(typeById == "btn-software-next"){
            if(count == 0){
                new Analytics().actionScrolSkills('software')
            }
            setCount(count+6 < language.software.skills.length ? count+6 : count)
        }else
            setCount(count <= 6 ? 0 : count-6)
    }

    return (
        <section className="section-skills">
            <div className="div-title">
                <h4 className="text title titleSkills">{language.title}</h4>
            </div>
            <div className="content-informationSkills">
                <div className="contentClassSkills content-software">
                    <h6 className="text title titleContent">{language.software.title}</h6>
                    <div className="contentSkilss">
                        <Skills Skills={language.software.skills} Count={count} Type='software' key={`software-${language.title}-${count}`}/>
                    </div>
                    <nav className="content-nav-button">
                        <button onClick={updateLineSoftware} id='btn-software-back' title={language.software.navSkills.backButtonText} className='text buttonNavSkills'>{language.software.navSkills.backButtonText}</button>
                        <button onClick={updateLineSoftware} id='btn-software-next' title={language.software.navSkills.nextButtonText} className='text buttonNavSkills'>{language.software.navSkills.nextButtonText}</button>
                    </nav>
                </div>
                <div className="contentClassSkills content-mechanic">
                    <h6 className="text title titleContent">{language.enginner.title}</h6>
                    <div className="contentSkilss">
                        <Skills Skills={language.enginner.skills} Count={0} Type='enginner' key={`enginner-${language.title}-${0}`}/>
                    </div>
                </div>
            </div>
        </section>
    )
}