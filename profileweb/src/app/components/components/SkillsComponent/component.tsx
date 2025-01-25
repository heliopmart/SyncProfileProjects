import { useState, useEffect, useRef } from 'react';
import 'animate.css';

export default function SkillsComponent({Skills, Count, Type} : {Skills: Array<string>, Count: number, Type: string}){
    const [isVisible, setIsVisible] = useState(false);
    const [animated, setAnimated] = useState<string | null>(null)
    const skillsRef = useRef(null);

    useEffect(() => {
        interface VisibilityChangeEntry extends IntersectionObserverEntry {
          isIntersecting: boolean;
        }

        const handleVisibilityChange = (entries: VisibilityChangeEntry[], observer: IntersectionObserver): void => {
          entries.forEach(entry => {
            if (entry.isIntersecting) {
              setIsVisible(true);
              observer.disconnect();
            } else {
              setIsVisible(false);
            }
          });
        };
    
        const observer = new IntersectionObserver(handleVisibilityChange, {
          root: null,
          threshold: 0.5,
        });
    
        if (skillsRef.current) {
          observer.observe(skillsRef.current);
        }
    
        return () => {
          if (skillsRef.current) {
            observer.unobserve(skillsRef.current);
          }
        };
    }, []);
    
    useEffect(() => {
        if (isVisible) {
            console.log("Animated")
            setAnimated("animate__bounceIn");
        }
    }, [isVisible]);
    
    return (
        <>
            {
                Skills.slice(Count, (Count+6)).map((key, index) => (
                    <span ref={skillsRef} key={Type+index+Count+animated} className={`text contentSpanSkills animate__animated  ${animated}`} dangerouslySetInnerHTML={{__html: key}}/>
                ))
            }
        </>
    )
}