## 🪟 **Projeto Windows App**

### Descrição
O aplicativo desktop foi desenvolvido para garantir sincronização entre arquivos locais e na nuvem, com foco em usabilidade e desempenho.

### Tecnologias Utilizadas
- **Linguagem**: C#
- **Framework**: .NET
- **Armazenamento de Arquivos**: API REST do Box.com

### Funcionalidades
- Sincronização em tempo real entre dispositivos e nuvem.
- Uploads automáticos ao detectar alterações locais.
- Gerenciamento offline com sincronização posterior.

### Como Rodar o Projeto Windows
1. Clone o repositório:
   ```bash
   git clone https://github.com/seu-usuario/syncprofileprojects.git
   ```
2. Navegue até a pasta do aplicativo Windows:
   ```bash
   cd syncprofileprojects/windows-app
   ```
3. Abra o projeto no Visual Studio:
   - Certifique-se de ter o SDK do .NET instalado.
   - Compile e execute o aplicativo.

--

## Estrutura de pastas 

```plaintext
windows-app/
├── bin/
├── obj/
├── Properties/
├── Models/              (Classes de modelo de dados - representando os dados da API)
│   └── Arquivo.cs        (Ex: Representa um arquivo a ser enviado ou recebido)
│   └── Projeto.cs         (Ex: Representa um projeto gerenciado)
├── Services/            (Lógica de negócio e interação com a API)
│   ├── Api/             (Classes específicas para comunicação com a API)
│   │   └── ApiClient.cs  (Cliente HTTP genérico para fazer requisições)
│   │   └── UploadService.cs (Lógica para uploads, usando ApiClient)
│   │   └── DownloadService.cs (Lógica para downloads, usando ApiClient)
│   ├── ProjectManageService.cs (Lógica para gerenciar projetos, orquestrando UploadService e DownloadService)
├── Views/               (Classes de interface do usuário - Forms, Janelas, etc.)
│   ├── UploadView.cs     (Formulário para upload)
│   ├── DownloadView.cs   (Formulário para download)
│   ├── ProjectManageView.cs (Formulário principal de gerenciamento)
├── Presenters/          (Lógica de apresentação - Intermediários entre Views e Services)
│   ├── UploadPresenter.cs
│   ├── DownloadPresenter.cs
│   ├── ProjectManagePresenter.cs
├── Utils/               (Classes utilitárias - Ex: tratamento de erros, logs)
│   └── ErrorHandler.cs
├── Program.cs           (Ponto de entrada da aplicação)
└── NomeDoProjeto.csproj
```


--------------------------

## TODO: 

-> Geraçao de logs de projetos locais | Precisa de sincronização em nuvem |
-> Criar função que cria o projeto | Sincronização com Banco de dados | 
-> Criar função que cria a pasta localmente do projeto | Sincronização em nuvem | 
-> Criar função que faz a sincronização em nuvem
   => Criar função que verifica qual arquivo precisa ser sincronizado 
   => Criar função Upload
   => Criar função Download 
-> Criar função que consome a API do Box.com
-> Criar função que consome a API do firebase