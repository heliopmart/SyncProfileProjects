using WindowsApp.Managers;
using WindowsApp.Utils;
using WindowsAppSync.Services.API;
using Box.Sdk.Gen;


namespace WindowsApp.Views
{
    public class SetApp : Form
    {
        private Button btnConfig = new Button();
        private Button btnProjects = new Button();
        private Button btnLogs = new Button();
        private Button btnStartSync = new Button();
        private Button btnStopSync = new Button();

        private Label lblTitle = new Label();
        private Label lblDescription = new Label();
        private Label lblSyncStatus = new Label();
        private BoxClient? _auth;
        private ProjectManager? _projectManager;
        private ProjectsApp? _projectsForm;

        // Notificação na bandeja do sistema
        private NotifyIcon trayIcon = new NotifyIcon();
         private ContextMenuStrip trayMenu = new ContextMenuStrip();

        public SetApp()
        {
            _ = InitializeComponentsAsync();
            SetTrayIcon(); // Função para configurar o ícone da bandeja
            MinimizeToTray(); // Função para minimizar para a bandeja
        }

        private async Task InitializeComponentsAsync()
        {
            _auth = await Authenticator.Auth();
            _projectManager = new ProjectManager(_auth);
            _projectsForm = new ProjectsApp(_auth, _projectManager);

            SetFormProperties();
            SetUpControls();
        }

        private void SetFormProperties()
        {
            this.Text = "Windows App Sync";
            this.Width = 500;
            this.Height = 500;
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void SetUpControls()
        {
            var mainPanel = CreateMainPanel();
            this.Controls.Add(mainPanel);

            lblTitle = CreateLabel("Bem-vindo ao Windows App Sync", new Font("Arial", 14, FontStyle.Bold));
            mainPanel.Controls.Add(lblTitle);

            lblDescription = CreateLabel("Escolha uma das opções abaixo:", new Font("Arial", 10, FontStyle.Regular));
            lblDescription.Margin = new Padding(0, 10, 0, 10);
            mainPanel.Controls.Add(lblDescription);

            btnConfig = CreateButton("Abrir Configurações", BtnConfig_Click);
            mainPanel.Controls.Add(btnConfig);

            btnProjects = CreateButton("Abrir Projetos", BtnProjects_Click);
            mainPanel.Controls.Add(btnProjects);

            btnLogs = CreateButton("Abrir Logs", BtnProjects_Click);
            mainPanel.Controls.Add(btnLogs);

            btnStartSync = CreateButton("Iniciar Sincronização", BtnStartSync_Click);
            mainPanel.Controls.Add(btnStartSync);

            btnStopSync = CreateButton("Parar Sincronização", BtnStopSync_Click);
            mainPanel.Controls.Add(btnStopSync);

            lblSyncStatus = new Label
            {
                Text = "Status da Sincronização: Parada",
                AutoSize = true,
                ForeColor = Color.Red
            };
            mainPanel.Controls.Add(lblSyncStatus);
        }

        private FlowLayoutPanel CreateMainPanel()
        {
            return new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(20),
                AutoScroll = true
            };
        }

        private Label CreateLabel(string text, Font font)
        {
            return new Label
            {
                Text = text,
                Font = font,
                AutoSize = true
            };
        }

        private Button CreateButton(string text, EventHandler clickEventHandler)
        {
            var button = new Button
            {
                Text = text,
                Width = 200,
                Height = 40
            };
            button.Click += clickEventHandler;
            return button;
        }

        // Evento de Configuração
        private void BtnConfig_Click(object? sender, EventArgs e)
        {
            var configForm = new ConfigApp();
            configForm.ShowDialog();
        }

        // Evento de Projetos
        private void BtnProjects_Click(object? sender, EventArgs e)
        {
            _projectsForm?.ShowDialog();
        }

        // Evento de Abrir Logs
        private void BtnLogs_Click(object? sender, EventArgs e)
        {
            MainForm logForm = new MainForm();
            logForm.Show();
        }

        // Evento para Iniciar a Sincronização
        private void BtnStartSync_Click(object? sender, EventArgs e)
        {
            HandleSyncAction(true);
        }

        // Evento para Parar a Sincronização
        private void BtnStopSync_Click(object? sender, EventArgs e)
        {
            HandleSyncAction(false);
        }

        private void HandleSyncAction(bool isStart)
        {
            var projectSelect = _projectsForm?.GetSelectedProject();
            if (projectSelect != null)
            {
                if (isStart)
                {
                    _ = OpenProject(_auth, _projectManager, projectSelect);
                    lblSyncStatus.Text = $"Status da Sincronização: Sincronizando com {projectSelect}";
                    lblSyncStatus.ForeColor = Color.Green;
                }
                else
                {
                    _ = CloseProject(_auth, _projectManager, projectSelect);
                    lblSyncStatus.Text = "Status da Sincronização: Parada";
                    lblSyncStatus.ForeColor = Color.Red;
                }
            }
            else
            {
                MessageBox.Show(isStart ? "Você não selecionou um projeto" : "Erro, Não foi possível encontrar o projeto");
            }
        }


        // Funcções de comunicação externa
        private static async Task OpenProject(BoxClient auth, ProjectManager projectManager, string nameProject)
        {
            if (!string.IsNullOrEmpty(nameProject))
            {
                await projectManager.OpenProjectForMonitory(auth, nameProject);
            }
        }

        private static async Task CloseProject(BoxClient auth, ProjectManager projectManager, string nameProject)
        {
            if (!string.IsNullOrEmpty(nameProject))
            {
                await projectManager.CloseProjectMonitory(auth, nameProject);
            }
        }

        // Função para configurar o ícone na bandeja
        private void SetTrayIcon()
        {
            trayMenu = new ContextMenuStrip(); // Usando ContextMenuStrip ao invés de ContextMenu
            trayMenu.Items.Add("Abrir", null, TrayOpen_Click);
            trayMenu.Items.Add("Sair", null, TrayExit_Click);

            trayIcon = new NotifyIcon
            {
                Icon = new Icon("./Resources/icone.ico"), // Ícone da bandeja
                ContextMenuStrip = trayMenu,
                Visible = true
            };
        }

        // Função para minimizar o app para a bandeja
        private void MinimizeToTray()
        {
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            trayIcon.ShowBalloonTip(3000, "Aplicativo Minimizando", "Aplicativo funcionando em segundo plano", ToolTipIcon.Info);
        }

        // Evento de clique no ícone da bandeja para abrir o app
        private void TrayOpen_Click(object? sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            trayIcon.Visible = false;
        }

        // Evento de clique no ícone da bandeja para sair
        private void TrayExit_Click(object? sender, EventArgs e)
        {
            trayIcon.Visible = false;
            Application.Exit();
        }

        // Substitua o método `OnFormClosing` para garantir que o app minimize ao invés de fechar
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                MinimizeToTray();
                SetTrayIcon();
            }
        }
    }
}
