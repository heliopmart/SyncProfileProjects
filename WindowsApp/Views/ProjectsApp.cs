using WindowsApp.Managers;
using WindowsApp.Helpers;
using Box.Sdk.Gen;
using WindowsApp.Models; // temp
using WindowsAppSync.Services.API;
using WindowsApp.Models.Class;

namespace WindowsApp.Views
{
    public partial class ProjectsApp : Form
    {
        // Inicialização dos campos obrigatórios no momento da declaração
        private ListBox lstProjects = new ListBox(); // Inicializado diretamente
        private Button btnAddProject = new Button(); // Inicializado diretamente
        private Label lblSelectedProject = new Label(); // Inicializado diretamente
        private string? selectedProject; // Variável para armazenar o projeto selecionado

        private BoxClient? _auth;
        private ProjectManager? _projectManager;

        private List<string> projects = new List<string>(); // Inicializa a lista como uma nova lista

        public ProjectsApp(BoxClient client, ProjectManager projectManager)
        {
            _auth = client;
            _projectManager = projectManager;
            InitializeComponent();
            _ = ListProjects(); // A chamada é assíncrona e segura
        }

        private async Task ListProjects()
        {
            var ProjectsData = await GetLogs.GetProjectsLogFile();
            if (ProjectsData?.LocalProjects != null)
            {
                projects = ProjectsData.LocalProjects.Values
                    .Select(p => p.Name)  // Extrai os nomes dos projetos
                    .ToList();            // Converte para uma lista de strings
            }
            else
            {
                throw new Exception("MAIN : ListProjects(), error: Metadata or Project Name not invalid.");
            }

            // Verificação adicional de nulidade antes de atribuir
            lstProjects.DataSource = projects ?? new List<string>();
        }

        private void InitializeComponent()
        {
            this.Text = "Gerenciar Projetos";
            this.Width = 400;
            this.Height = 300;
            this.StartPosition = FormStartPosition.CenterScreen;

            lstProjects = new ListBox()
            {
                Width = 250,
                Height = 150,
                Location = new Point(20, 20)
            };
            lstProjects.SelectedIndexChanged += LstProjects_SelectedIndexChanged; // Evento de seleção
            this.Controls.Add(lstProjects);

            // Garantindo que o DataSource seja atribuído de forma segura
            lstProjects.DataSource = projects ?? new List<string>();

            lblSelectedProject = new Label()
            {
                Text = "Projeto Selecionado: Nenhum", // Texto inicial
                Location = new Point(20, 180),
                AutoSize = true
            };
            this.Controls.Add(lblSelectedProject);  // Adiciona o label ao formulário

            // Botão para adicionar um projeto
            btnAddProject = new Button()
            {
                Text = "Adicionar Projeto",
                Width = 200,
                Height = 40,
                Location = new Point(20, 210)
            };
            btnAddProject.Click += BtnAddProject_Click; // Evento de clique para adicionar
            this.Controls.Add(btnAddProject);
        }

        // Evento de clique do botão "Adicionar Projeto"
        private void BtnAddProject_Click(object? sender, EventArgs e)
        {
            // Solicitar ao usuário o nome do novo projeto
            string NameProject = Microsoft.VisualBasic.Interaction.InputBox("Digite o nome do novo projeto", "Adicionar Projeto", "");
            if (!string.IsNullOrEmpty(NameProject))
            {
                _ = AddProject(NameProject);
                projects.Add(NameProject);  // Adiciona o novo projeto à lista
                lstProjects.DataSource = null; // Redefine o DataSource
                lstProjects.DataSource = projects; // Atualiza a lista
            }
        }

        // Evento de seleção de um item na lista
        private void LstProjects_SelectedIndexChanged(object? sender, EventArgs e)
        {
            // Verifique se algum item foi selecionado (caso contrário, seleciona um item vazio)
            if (lstProjects.SelectedItem != null)
            {
                // Atualiza a variável 'selectedProject' com o projeto selecionado
                selectedProject = lstProjects.SelectedItem.ToString();

                // Exibe o projeto selecionado no label
                lblSelectedProject.Text = $"Projeto Selecionado: {selectedProject}";
            }
        }

        // Função para acessar o projeto selecionado em outra parte do aplicativo
        public string GetSelectedProject()
        {
            return selectedProject ?? string.Empty; // Retorna uma string vazia caso seja nulo
        }

        private async Task AddProject(string NameProject)
        {
            if (_projectManager != null)
            {
                if (!string.IsNullOrEmpty(NameProject))
                {
                    try
                    {
                        await _projectManager.AddProject(_auth, NameProject);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"ProjectsApp : AddProject(), Erro: {ex}");
                    }
                }
            }
        }
    }
}
