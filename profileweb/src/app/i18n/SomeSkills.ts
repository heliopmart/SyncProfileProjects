export default interface Language {
  title: string,
  software: {
    title: string,
    skills: string[]
    navSkills: {
      nextButtonText: string,
      backButtonText: string
    }
  }
  enginner: {
    title: string,
    skills: string[]
  }
}
  