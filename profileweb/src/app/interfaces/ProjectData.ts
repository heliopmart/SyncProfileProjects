export default interface ProjectData{
    software?: ProjectDataSoftware | null,
    mechanic?: ProjectDataMechanic | null 
}

export interface ProjectDataSoftware{
    repo_id: string
    gitHubData: GithubData,
    languages: string[] | null
}


export interface GithubData{
    name: string,
    html_url: string,
    description: string,
    download_url: string, 
    languages_url: string
    created_at: string,
    pushed_at: string,
    creationTime: number | null
}

export interface ProjectDataMechanic{
    id: string,
    name: string,
    url_readme: string,
    FolderId: string,
    DateTime: string,
    AsyncTime: string,
    creationTime: number | null
    metaDataProject?: MetadataProject
}

export interface MetadataProject{
    description: string,
    url_image: string,
    public_files: Public_files[]
}

export type Public_files = {
    name: string,
    share: string,
    extention?: string
}

export interface FirebaseMetadataDocument{
    AsyncTime: string,
    DateTime: string,
    Device: string,
    Description?: string | null
    FolderId: string,
    url_readme: string,
    Id: string,
    Name: string,
    Status: number
    metaDataProject?: MetadataProject
}