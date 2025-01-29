import "./style.scss"
import Image from 'next/image'

const ImageCertificateIcon = "/images/certificateIcon.svg";
const ImageRealTimeSync = "/images/realTimeSync.svg";

export default function SectionAboutProjectImage(){
    return (
        <div className="SectionAboutProjectImage">
            <Image src={ImageCertificateIcon} alt="certificate" id="certificate" width={520} height={330} />
            <Image src={ImageRealTimeSync} alt="realTimeSync" id="realTimeSync" width={370} height={240} />
        </div>
    )
}