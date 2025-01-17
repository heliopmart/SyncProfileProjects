using System;
using System.Collections.Generic;
using System.Windows.Forms;
using WindowsApp.Helpers;

namespace WindowsApp.Views
{
    public class ConfigApp : Form
    {
        private CheckBox cbDev;
        private TextBox txtDevToken;
        private TextBox txtDefaultPath;
        private NumericUpDown numSyncTime;
        private Button btnSave;
        private APPConfig _config;

        public ConfigApp()
        {
            _config = GetDataConfig();
            if(_config != null){
                InitializeComponent();
            }else{
                MessageBox.Show($"Config is null");
            }
        }

        private APPConfig GetDataConfig(){
            return ConfigHelper.Instance.GetConfig();
        }

        private void InitializeComponent()
        {
            // Definir propriedades do formul�rio
            this.Text = "Configura��es do App";
            this.Width = 400;
            this.Height = 300;
            this.StartPosition = FormStartPosition.CenterScreen; // Centraliza a janela na tela

            // Painel principal para centralizar os controles
            var mainPanel = new FlowLayoutPanel()
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(20),
                AutoScroll = true // Permite rolar se necess�rio
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
                Checked = _config.Development,
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
                Text = _config.APIConfigs.Token,
                Width = 250
            };
            mainPanel.Controls.Add(labelDevToken);
            mainPanel.Controls.Add(txtDevToken);

            // Label e Campo de texto para DefaultPath
            var labelDefaultPath = new Label()
            {
                Text = "Caminho Padr�o:",
                AutoSize = true
            };
            txtDefaultPath = new TextBox()
            {
                PlaceholderText = "Informe o caminho padr�o",
                Text = _config.DefaultPathForProjects ?? null,
                Width = 250
            };
            mainPanel.Controls.Add(labelDefaultPath);
            mainPanel.Controls.Add(txtDefaultPath);

            // Label e Campo num�rico para Tempo de Sincroniza��o
            var labelSyncTime = new Label()
            {
                Text = "Tempo de Sincroniza��o (minutos):",
                AutoSize = true
            };
            numSyncTime = new NumericUpDown()
            {
                Minimum = 1,
                Maximum = 3600,
                Value = _config.SyncInterval,
                Width = 250
            };
            mainPanel.Controls.Add(labelSyncTime);
            mainPanel.Controls.Add(numSyncTime);

            // Bot�o para salvar as configura��es
            btnSave = new Button()
            {
                Text = "Salvar Configura��es",
                AutoSize = true
            };
            btnSave.Click += BtnSave_Click;
            mainPanel.Controls.Add(btnSave);
        }

        // ... c�digo existente ...

        private async void BtnSave_Click(object sender, EventArgs e) // Adicione 'async' aqui
        {
            if (sender is Button button)
            {
                // Aqui voc� pode salvar ou aplicar as configura��es
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

                if (await ModifyAppSetting.ChangeAppSettings(settings)) // Corrigido para 'settings'
                {
                    MessageBox.Show($"appsettings atualizado com sucesso!");
                }
                else
                {
                    MessageBox.Show($"Erro na atualiza��o do appsettings.");
                }

                this.Close();  // Fecha o formul�rio de configura��es
            }
        }
    }
}
