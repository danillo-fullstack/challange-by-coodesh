using challange_by_coodesh.Views;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace challange_by_coodesh
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MenuPessoa_Click(object sender, RoutedEventArgs e)
        {
            PessoaView pessoaView = new PessoaView();
            pessoaView.ShowDialog();
        }

        private void MenuProduto_Click(object sender, RoutedEventArgs e)
        {
            ProdutoView produtoView = new ProdutoView();
            produtoView.ShowDialog();
        }
    }
}