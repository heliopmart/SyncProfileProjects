import { getAnalytics, logEvent, isSupported } from "firebase/analytics";
import { fetchAndActivate, getValue, getRemoteConfig } from "firebase/remote-config";
import { app} from '@/app/api/firebase/Auth'

async function _analytics(){
    if (typeof window !== "undefined" && await isSupported()) {
        return getAnalytics(app);
    }else{
        return false
    }
}

function _remoteConfig(){
    if (typeof window !== "undefined") {
        return getRemoteConfig(app); 
    }else{
        return false
    }
}

export default class Analytics {
    actionChangeLayoutLog(whatsButton: string) {
        const analytics = _analytics()
        analytics.then(value => {
            if(!value){
                return 
            }
    
            logEvent(value, `button-change-layout-${whatsButton}`);
        })
    }

    actionDeviceTypeAccess(deviceType: string) {
        const analytics = _analytics()
        analytics.then(value => {
            if(!value){
                return 
            }
    
            logEvent(value, `access-in-${deviceType}`);
        })
    }

    actionLanguageSelect(language: string) {
        const analytics = _analytics()
        analytics.then(value => {
            if(!value){
                return 
            }
    
            logEvent(value, `language-${language}`);
        })
    }

    actionScrolSkills(type: string) {
        const analytics = _analytics()
        analytics.then(value => {
            if(!value){
                return 
            }
    
            logEvent(value, `scroll-skills-${type}`);
        })
    }

    actionResumeDownload() {
        const analytics = _analytics()
        analytics.then(value => {
            if(!value){
                return 
            }
    
            logEvent(value, `download-click-resume`);
        })
        
    }

    async getRemoteResumeLink() {
        const remoteConfig = _remoteConfig()
        if(!remoteConfig){
            return 
        }
        
        try {
            await fetchAndActivate(remoteConfig);
            const link = getValue(remoteConfig, 'link_url_resume').asString();

            if (link) {
                return link
            } else {
               return "#"
            }
        } catch{
            return "#"
        }
    }
}