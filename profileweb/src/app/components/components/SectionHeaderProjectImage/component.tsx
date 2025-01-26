import "./style.scss"
import Image from 'next/image'

const ImageFolderFilesManIcon = "/images/folderFilesManIcon.svg";
const ImageTeamUpIcon = "/images/teamUpIcon.svg";

export default function SectionHeaderProjectImage(){
    return (
        <div className="headerProject_images">
            <Image src={ImageFolderFilesManIcon} alt="Folder File Icon" id="forder_file_man" width={520} height={330} />
            <Image src={ImageTeamUpIcon} alt="Team Up" id="team_up" width={370} height={240} />
        </div>
    )
}