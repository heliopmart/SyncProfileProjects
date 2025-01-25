import "./style.scss"
import Image from 'next/image'

const ImageProgramming = "/images/programming.svg";
const ImageBridge = "/images/bridge.svg";

export default function SectionProjectViewImage(){
    return (
        <div className="projectView_images">
            <Image src={ImageProgramming} alt="software programer" id="software_programer" width={520} height={330} />
            <Image src={ImageBridge} alt="bridge" id="bridge" width={370} height={240} />
        </div>
    )
}