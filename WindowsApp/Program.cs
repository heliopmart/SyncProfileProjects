using WindowsApp.Views;


namespace WindowsApp
{
    public class TestApp : Form
    {
        [STAThread] // Necessário para o Windows Forms
        public static async Task Main(string[] args)
        {
            try
            {
                Application.Run(new SetApp());  // Inicia o aplicativo
            }
            catch
            {
                Console.WriteLine("Erro ao compilar");
            }
        }
    }
}
