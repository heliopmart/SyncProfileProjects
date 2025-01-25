'use client'
// TODO: REFATORAR HEADER COMPONENT
import React, {useEffect, useState} from 'react'
import { TypeAnimation } from 'react-type-animation';
import 'animate.css';
import "./style.scss"

import HeaderInterface from '@/app/i18n/Header'
import languageBr from '@/app/i18n/Header_br.json'
import languageUs from '@/app/i18n/Header_us.json'

type Sentence = string | null; 
type ArrSentence = Array<string>; 
type isWriting = boolean; 
type speed = number; 
type direction = "left" | "right";
type ResIsWriteFunction = (res: boolean) => void;
type ResCreatingSentence = (res: Sentence) => void;
type OptionSelect = (res: React.MouseEvent<HTMLButtonElement>) => void;
type OptionLanguageSelect = (res: React.ChangeEvent<HTMLSelectElement>) => void;
type LanguageSelect = (res: string) => void;

interface CuriosityWriterProps {
    sentence: Sentence;
    isWriting: isWriting;
    speed: number;
    direction: direction;
    resCreatingSentence: ResCreatingSentence;
    resIsWrite: ResIsWriteFunction;
}

const WellcomeComponentAnimation = ({ Text }: { Text: string }) => {
    return (
      <TypeAnimation
        sequence={[Text, 1000]}
        wrapper="h6"
        cursor={true}
        key={Text}
        repeat={0}
        className={"text titleWellcome"}
      />
    );
};

const CuriosityWriter: React.FC<CuriosityWriterProps> = ({sentence, isWriting, speed, direction, resIsWrite, resCreatingSentence}) => {
    if(sentence==null){
        return (<span>Puts!</span>);
    }

    const sequence = [];
    let currentSentence = '';
    if(direction == 'right' || isWriting == false){
        const words = sentence.split(' ');
        
        sequence.push(() => resIsWrite(true));
    
        words.forEach((word, index) => {
            currentSentence += (index === 0 ? '' : ' ') + word;        
            sequence.push(currentSentence);
            sequence.push(speed);
            sequence.push(() => resCreatingSentence(currentSentence));
        });

        sequence.push(() => resIsWrite(false));
    }else{
        const words = sentence.split(' ');
        sequence.push(() => resIsWrite(true));
        for (let i = words.length - 1; i >= 0; i--) {
            currentSentence = words.slice(0, i + 1).join(' ');  
            sequence.push(() => resCreatingSentence(currentSentence));
            sequence.push(speed); 
            sequence.push(currentSentence);
        }
        sequence.push(() => resIsWrite(false)); 
    }

    return (
        <TypeAnimation
            sequence={sequence}
            wrapper="p"
            cursor={true}
            repeat={0}
            className={"text pWellcome"}
            key={sentence+direction+speed}
        />
    )
}

export default function HeaderIndex({optionSelect, languageSelect}:{optionSelect:OptionSelect, languageSelect:LanguageSelect}){
    const [flag, setFlag] = useState<string>("br");
    const [sentence, setSentence] = useState<Sentence>(null);
    const [creatingSentence, setCreatingSentence] = useState<ArrSentence>([]);
    const [isWriting, setIsWriting] = useState<isWriting>(false);
    const [coolDown, setCoolDown] = useState<boolean>(true);
    const [speed, setSpeed] = useState<speed>(5);
    const [direction, setDirection] = useState<direction>('right');
    const [language, setLanguage] = useState<HeaderInterface>(languageBr);

    const updateIsWrite:ResIsWriteFunction = (res:isWriting) => {
        setIsWriting(res);
    }
    const updateCreatingSentence:ResCreatingSentence = (res:Sentence) => {
        if(res !== null)
            setCreatingSentence([...creatingSentence, res]);
    }
    const OptionLanguageSelect:OptionLanguageSelect = (event: React.ChangeEvent<HTMLSelectElement>) => {
        const flag = event.target.value
        setFlag(flag)
        languageSelect(flag)
    }

    function CallCuriosityWriter(_id:string){
        if(coolDown){
            return;
        }
        const id = _id as keyof typeof language.header.curiosity
        const randomKeyCuriosity = Math.floor(Math.random() * ((language.header.curiosity[id].length)));
        if(isWriting){
            setSpeed(1);
            setDirection('left');
            setSentence(creatingSentence.join(" "))
            setTimeout(() => {
                setSentence(language.header.curiosity[id][randomKeyCuriosity])
                setDirection('right');
                setSpeed(5);
            },100)
        }else{
            setDirection('right');
            setSentence(language.header.curiosity[id][randomKeyCuriosity])
            setSpeed(1);
        }
    }

    // TODO: Funciona, porém eu queria que ele apagasse oque escreveu para começara escrever outra coisa
    const handleMouseButton = (event : React.MouseEvent<HTMLButtonElement>) => {
        const id = event.currentTarget.id;
        CallCuriosityWriter(id);
    }
    const handleMouseDiv = (event : React.MouseEvent<HTMLDivElement>) => {
        const id = event.currentTarget.id;
        CallCuriosityWriter(id);
    }

    useEffect(() => {
        const handleLoad = () => {
            setCoolDown(false);
        };
    
        if (document.readyState === 'complete') {
          handleLoad();
        } else {
          window.onload = handleLoad;
        }
    },[coolDown])

    useEffect(() => {
        switch(flag){
            case "br": 
                setLanguage(languageBr)
                break;
            default:
                setLanguage(languageUs)
                break;
        }
        setSentence(null);
        
    },[flag])

    return (
        <header>
            <section className="sectionHead">
                <div id="div-name" onMouseEnter={handleMouseDiv}>
                    <h1 className='name text spantext'>{language.header.head.name}</h1>
                </div>
                <div id="div-language">
                    <label htmlFor="optionlanguage">
                        <div id="iconFlag" className={flag == "br" ? "flag-br" : "flag-us"}></div>
                    </label>
                    <select id="optionlanguage" onChange={OptionLanguageSelect} title={language.header.head.selectTittle} className='spantext optionlanguage'>
                        <option value={'br'}>Português</option>
                        <option value={'us'}>English</option>
                    </select>
                </div>
            </section>
            <section className='animationSectionWellcome sectionWellcome'>
                <div className="div-left">
                    <div className="div-titileWellcome">
                        <WellcomeComponentAnimation key={"Text-Version"+flag} Text={language.header.wellcome.titleFirstAcess[0]}/>
                    </div>
                    <div className="div-subtextWellcome">
                        {sentence != null ? (
                            <CuriosityWriter sentence={sentence} direction={direction} isWriting={isWriting} speed={speed} key={"CuriosityWriter_Wellcome"+flag} resIsWrite={updateIsWrite} resCreatingSentence={updateCreatingSentence}/>
                        ):(
                            <p className='text pWellcome' dangerouslySetInnerHTML={{ __html: language.header.wellcome.subtitle}}/>
                        )}
                    </div>
                </div>
                <div className="div-right">
                    <div className="imageWellcome" id="imageWellcome" onMouseEnter={handleMouseDiv}></div>
                </div>
            </section>
            <section className='sectionButtons'>
                <button onClick={optionSelect} onMouseEnter={handleMouseButton} title={language.header.buttons.story} className='text btnOptions' id='story'>
                    <svg>
                        <rect x="0" y="0" fill="none" width="100%" height="100%"/>
                    </svg>
                    {language.header.buttons.story}
                </button>
                <button onClick={optionSelect} onMouseEnter={handleMouseButton} title={language.header.buttons.recruiter} className='text btnOptions' id='recruiter'>
                    <svg>
                        <rect x="0" y="0" fill="none" width="100%" height="100%"/>
                    </svg>
                    {language.header.buttons.recruiter}
                </button>
                <button onClick={optionSelect} onMouseEnter={handleMouseButton} title={language.header.buttons.just} className='text btnOptions' id='just'>
                    <svg>
                        <rect x="0" y="0" fill="none" width="100%" height="100%"/>
                    </svg>
                    {language.header.buttons.just}
                </button>
            </section>
        </header>
    );
}