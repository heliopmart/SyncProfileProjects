using System;
using System.Collections.Generic;
using System.Windows.Forms;
using WindowsApp.Helpers;

namespace WindowsApp.Views
{
    public class ConfigApp : Form
    {
        // Torne os campos não anuláveis ou inicialize-os diretamente no construtor
        private CheckBox cbDev = new CheckBox();
        private TextBox txtDevToken = new TextBox();
        private TextBox txtDefaultPath = new TextBox();
        private NumericUpDown numSyncTime = new NumericUpDown();
        private Button btnSave = new Button();
        private APPConfig? _config;

        public ConfigApp()
        {
            _config = GetDataConfig();
            if (_config != null)
            {
                InitializeComponent();
            }
            else
            {
                MessageBox.Show("Config is null");
            }
        }

        private APPConfig GetDataConfig()
        {
            return ConfigHelper.Instance.GetConfig();
        }

        private void InitializeComponent()
        {
            // Definir propriedades do formulário
            this.Text = "Configurações do App";
            this.Width = 400;
            this.Height = 300;
            this.StartPosition = FormStartPosition.CenterScreen; // Centraliza a janela na tela

            // Painel principal para centralizar os controles
            var mainPanel = new FlowLayoutPanel()
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(20),
                AutoScroll = true // Permite rolar se necessário
            };
            this.Controls.Add(mainPanel);

            // Label e Checkbox para Modo de Desenvolvimento
            var labelDevMode = new Label()
            {
                Text = "Modo Desenvolvimento:",
                AutoSize = true
            };
            cbDev = new CheckBox()
            {
                Text = "Ativar Modo Dev",
                Checked = _config != null && _config.Development,
                AutoSize = true
            };
            mainPanel.Controls.Add(labelDevMode);
            mainPanel.Controls.Add(cbDev);

            // Label e Campo de texto para DevToken
            var labelDevToken = new Label()
            {
                Text = "DevToken:",
                AutoSize = true
            };
            txtDevToken = new TextBox()
            {
                PlaceholderText = "Informe o DevToken",
                Text = _config?.APIConfigs.Token ?? string.Empty,
                Width = 250
            };
            mainPanel.Controls.Add(labelDevToken);
            mainPanel.Controls.Add(txtDevToken);

            // Label e Campo de texto para DefaultPath
            var labelDefaultPath = new Label()
            {
                Text = "Caminho Padrão:",
                AutoSize = true
            };
            txtDefaultPath = new TextBox()
            {
                PlaceholderText = "Informe o caminho padrão",
                Text = _config?.DefaultPathForProjects ?? string.Empty,
                Width = 250
            };
            mainPanel.Controls.Add(labelDefaultPath);
            mainPanel.Controls.Add(txtDefaultPath);

            // Label e Campo numérico para Tempo de Sincronização
            var labelSyncTime = new Label()
            {
                Text = "Tempo de Sincronização (minutos):",
                AutoSize = true
            };
            numSyncTime = new NumericUpDown()
            {
                Minimum = 1,
                Maximum = 3600,
                Value = _config?.SyncInterval ?? 5,
                Width = 250
            };
            mainPanel.Controls.Add(labelSyncTime);
            mainPanel.Controls.Add(numSyncTime);

            // Botão para salvar as configurações
            btnSave = new Button()
            {
                Text = "Salvar Configurações",
                AutoSize = true
            };
            btnSave.Click += BtnSave_Click;
            mainPanel.Controls.Add(btnSave);
        }

        private async void BtnSave_Click(object? sender, EventArgs e)
        {
            if (sender is Button button)
            {
                bool devMode = cbDev.Checked;
                string devToken = txtDevToken.Text;
                string defaultPath = txtDefaultPath.Text;
                int syncTime = (int)numSyncTime.Value;

                var settings = new ChangeSettings
                {
                    DefaultPathForProjects = defaultPath,
                    Development = devMode,
                    SyncInterval = syncTime,
                    Token = devToken
                };

                if (await ModifyAppSetting.ChangeAppSettings(settings))
                {
                    MessageBox.Show("appsettings atualizado com sucesso!");
                }
                else
                {
                    MessageBox.Show("Erro na atualização do appsettings.");
                }

                this.Close();  // Fecha o formulário de configurações
            }
        }
    }
}
