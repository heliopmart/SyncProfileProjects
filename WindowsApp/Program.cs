using WindowsApp.Views;
namespace WindowsApp
{
    public class TestApp : Form
    {
        [STAThread] // Necessário para o Windows Forms
        public static void Main(string[] args)
        {
            try
            {
                Application.Run(new SetApp()); 
            }
            catch
            {
                Console.WriteLine("Erro ao compilar");
            }
        }
    }
}
