using challange_by_coodesh.Models;
using challange_by_coodesh.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace challange_by_coodesh.Views
{
    /// <summary>
    /// Lógica interna para PedidoView.xaml
    /// </summary>
    public partial class PedidoView : Window
    {
        private Pessoa _pessoa;
        private ProdutoService _produtoService;
        private List<ItemPedido> _itensPedido = new List<ItemPedido>();

        public PedidoView(Pessoa pessoa)
        {
            try
            {
                InitializeComponent();
                _pessoa = pessoa;
                _produtoService = new ProdutoService();
                CarregarDadosPessoa();
                CarregarProdutos();
            }
            catch (Exception ex) 
            { 
                MessageBox.Show($"Ocorreu um erro ao carregar os dados do pedido: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CarregarDadosPessoa()
        {
            txtClienteId.Text = _pessoa.Id.ToString();
            txtClienteNome.Text = _pessoa.Nome;
            txtClienteCpf.Text = _pessoa.CPF;
        }

        private void CarregarProdutos()
        {
            cbProdutos.ItemsSource = _produtoService.GetProdutos();
            cbProdutos.DisplayMemberPath = "ProdutoDescricao";
            cbProdutos.SelectedValuePath = "Id";
            txtDataVenda.Text = DateTime.Now.ToString("dd/MM/yyyy");
        }

        private void btnAtualizarItem_Click(object sender, RoutedEventArgs e)
        {
            // Lógica para remover item do pedido
        }

        private void btnRemoverItem_Click(object sender, RoutedEventArgs e)
        {
            // Lógica para remover item do pedido
        }

        private void cbStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Lógica para alterar status do pedido
        }

        private void cbSituacao_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Lógica para alterar situação do pedido
        }

        private void btnAdicionarProduto_Click(object sender, RoutedEventArgs e)
        {
            if (cbProdutos.SelectedItem == null)
            {
                MessageBox.Show(
                    "Selecione um produto.");

                return;
            }

            if (!int.TryParse(
                txtQuantidade.Text,
                out int quantidade))
            {
                MessageBox.Show(
                    "Informe uma quantidade válida.");

                txtQuantidade.Focus();

                return;
            }

            Produto produto =
                (Produto)cbProdutos.SelectedItem;

            ItemPedido item = new ItemPedido
            {
                ProdutoId = produto.Id,
                NomeProduto = produto.Nome,
                Quantidade = quantidade,
                ValorUnitario = produto.Valor
            };

            _itensPedido.Add(item);

            dgItensPedido.ItemsSource = null;

            dgItensPedido.ItemsSource = _itensPedido;

            AtualizarValorTotal();

            txtQuantidade.Clear();

            cbProdutos.SelectedIndex = -1;
        }

        private void AtualizarValorTotal()
        {
            decimal total = _itensPedido.Sum(item => item.ValorTotal);
            txtValorTotal.Text = total.ToString("C");
        }
    }
}
