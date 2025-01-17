using WindowsApp.Views;
namespace WindowsApp
{
    public class TestApp : Form
    {
        [STAThread] // Necessário para o Windows Forms
        public static void Main(string[] args)
        {
            /*
                TODO:
                Criar uma função que grave dos projetos no firebase 
                Criar uma função que resgate os dados dos projetos do firebase
                Criar uma função que atualize os dados do metadata.yaml com os dados do firebase
                Criar uma função capaz de sincronizar os dados locais do metadata com os do firebase

                Verificar a funcionalidade metatada.yaml quando ele próprio não existe
            */
            try
            {
                Application.Run(new SetApp()); 
            }
            catch
            {
                MessageBox.Show("Erro ao inicializar");
            }
        }
    }
}
