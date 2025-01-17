using WindowsApp.Views;
using WindowsApp.Managers;
using WindowsAppSync.Services.API;
using Box.Sdk.Gen;


namespace WindowsApp.Views
{
    public class SetApp : Form
    {
        private Button btnConfig = new Button();
        private Button btnProjects = new Button();
        private Button btnStartSync = new Button();
        private Button btnStopSync = new Button();

        private Label lblTitle = new Label();
        private Label lblDescription = new Label();
        private Label lblSyncStatus = new Label();
        private BoxClient? _auth;
        private ProjectManager? _projectManager;
        private ProjectsApp? _projectsForm;

        public SetApp()
        {
            _ = InitializeComponent();
        }

        private async Task InitializeComponent()
        {
            _auth = await Authenticator.Auth();
            _projectManager = new ProjectManager(_auth);
            _projectsForm = new ProjectsApp(_auth, _projectManager);

            // Definir propriedades do formul�rio
            this.Text = "Windows App Sync";
            this.Width = 500;
            this.Height = 400;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Painel principal para centralizar os controles
            var mainPanel = new FlowLayoutPanel()
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(20),
                AutoScroll = true
            };
            this.Controls.Add(mainPanel);

            // T�tulo da janela
            lblTitle = new Label()
            {
                Text = "Bem-vindo ao Windows App Sync",
                Font = new System.Drawing.Font("Arial", 14, System.Drawing.FontStyle.Bold),
                AutoSize = true
            };
            mainPanel.Controls.Add(lblTitle);

            // Descri��o
            lblDescription = new Label()
            {
                Text = "Escolha uma das op��es abaixo:",
                AutoSize = true,
                Margin = new Padding(0, 10, 0, 10)
            };
            mainPanel.Controls.Add(lblDescription);

            // Bot�o para abrir o formul�rio de configura��es
            btnConfig = new Button()
            {
                Text = "Abrir Configurações",
                Width = 200,
                Height = 40
            };
            btnConfig.Click += BtnConfig_Click;
            mainPanel.Controls.Add(btnConfig);

            // Bot�o para abrir o formul�rio de projetos
            btnProjects = new Button()
            {
                Text = "Abrir Projetos",
                Width = 200,
                Height = 40
            };
            btnProjects.Click += BtnProjects_Click;
            mainPanel.Controls.Add(btnProjects);

            // Bot�o para iniciar a sincroniza��o
            btnStartSync = new Button()
            {
                Text = "Iniciar Sincronização",
                Width = 200,
                Height = 40
            };
            btnStartSync.Click += BtnStartSync_Click;
            mainPanel.Controls.Add(btnStartSync);

            // Bot�o para parar a sincroniza��o
            btnStopSync = new Button()
            {
                Text = "Parar Sincroniza��o",
                Width = 200,
                Height = 40
            };
            btnStopSync.Click += BtnStopSync_Click;
            mainPanel.Controls.Add(btnStopSync);

            // Label de status de sincroniza��o
            lblSyncStatus = new Label()
            {
                Text = "Status da Sincroniza��o: Parada",
                AutoSize = true,
                ForeColor = System.Drawing.Color.Red
            };
            mainPanel.Controls.Add(lblSyncStatus);
        }

        // Eventos dos bot�es
        private void BtnConfig_Click(object sender, EventArgs e)
        {
            var configForm = new ConfigApp();
            configForm.ShowDialog();  // Exibe o formul�rio de configura��es
        }

        private void BtnProjects_Click(object sender, EventArgs e)
        {
            
            _projectsForm.ShowDialog();  // Exibe o formul�rio de projetos
        }

        private void BtnStartSync_Click(object sender, EventArgs e)
        {
            var projectSelect = _projectsForm?.GetSelectedProject();
            if(projectSelect != null){
                
                _ = OpenProject(_auth, _projectManager, projectSelect);

                lblSyncStatus.Text = $"Status da Sincronização: Sincronizando com {projectSelect}";
                lblSyncStatus.ForeColor = System.Drawing.Color.Green;
            }else{
                MessageBox.Show("Você não selecionou um projeto");
            }
        }

        private void BtnStopSync_Click(object sender, EventArgs e)
        {
            var projectSelect = _projectsForm?.GetSelectedProject();
            if(projectSelect != null){
                _ = CloseProject(_auth, _projectManager, projectSelect);
                lblSyncStatus.Text = "Status da Sincronização: Parada";
                lblSyncStatus.ForeColor = System.Drawing.Color.Red;
            }else{
                MessageBox.Show("Erro, Não foi possivel encontrar o projeto");
            }
        }

        private static async Task OpenProject(BoxClient auth, ProjectManager projectManager, string NameProject){
            if(NameProject != null){
                await projectManager.OpenProjectForMonitory(auth, NameProject);
            }
        }

        private static async Task CloseProject(BoxClient auth, ProjectManager projectManager, string NameProject){
            if(NameProject != null){
                await projectManager.CloseProjectMonitory(auth, NameProject);
            }
        }
    }
}
