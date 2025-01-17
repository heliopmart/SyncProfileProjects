using System;
using System.IO;
using System.Windows.Forms;

namespace WindowsApp.Views
{
    public partial class MainForm : Form
    {
        private TextBox txtLog = new TextBox(); // Inicializado diretamente
        private System.Windows.Forms.Timer timer; // Especificamente 'System.Windows.Forms.Timer' para evitar ambiguidade
        private StringWriter stringWriter = new StringWriter(); // StringWriter para capturar a saída do Console

        public MainForm()
        {
            InitializeComponent();
            timer = new System.Windows.Forms.Timer(); // Initialize the timer
            InitializeConsoleRedirection();
        }

        private void InitializeComponent()
        {
            this.txtLog = new TextBox
            {
                Multiline = true,
                Width = 400,
                Height = 200,
                Location = new System.Drawing.Point(20, 20),
                ScrollBars = ScrollBars.Vertical
            };

            this.Controls.Add(txtLog);

            this.Text = "Console Output to TextBox";
            this.Width = 500;
            this.Height = 300;
        }

        private void InitializeConsoleRedirection()
        {
            // Cria um StringWriter para capturar a saída do Console
            stringWriter = new StringWriter();
            Console.SetOut(stringWriter); // Redireciona a saída do Console para o StringWriter

            // Cria um timer para atualizar o TextBox com os logs em tempo real
            timer = new System.Windows.Forms.Timer
            {
                Interval = 1000 // Atualiza o TextBox a cada 1 segundo
            };
            timer.Tick += (sender, e) => UpdateLog();
            timer.Start();
        }

        private void UpdateLog()
        {
            // Captura o texto atual do StringWriter e exibe no TextBox
            txtLog.Text = stringWriter.ToString();
        }

        // Exemplo de uso de Console.WriteLine
        private void SimulateLog()
        {
            Console.WriteLine("Este é um log de exemplo.");
            Console.WriteLine("Mais uma linha de log.");
        }

        // Exemplo de chamada para gerar logs
        private void MainForm_Load(object sender, EventArgs e)
        {
            SimulateLog(); // Gera logs ao carregar o formulário
        }
    }
}
