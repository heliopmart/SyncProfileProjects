import "./style.scss"
import Image from 'next/image'

const ImageSoftwareEnginner = "/images/Image_software_enginner.svg";
const ImageLockData = "/images/Image_Lock_Data.svg";

export default function SectionHeaderImage(){
    return (
        <div className="header_images">
            <Image src={ImageSoftwareEnginner} alt="Software Enginner" id="software_enginner" width={520} height={330} />
            <Image src={ImageLockData} alt="Lock Data" id="lock_data" width={370} height={240} />
        </div>
    )
}