export default interface Language {
    header: {
      head: {
        name: string;
        selectTittle: string;
      };
      wellcome: {
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
        imageWellcome: string[];
        "div-name": string[];
      };
    };
  }
  