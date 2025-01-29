export default interface Language {
    header: {
      head: {
        name: string;
        selectTittle: string;
      };
      Welcome: {
        titleFirstAcess: string[];
        title: string[];
        subtitle: string;
      };
      buttons: {
        story: string;
        recruiter: string;
        just: string;
      };
      curiosity: {
        story: string[];
        recruiter: string[];
        just: string[];
        imageWelcome: string[];
        "div-name": string[];
      };
    };
  }
  