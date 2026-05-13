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
        private Pessoa _pessoa = new Pessoa();
        private ProdutoService _produtoService = new ProdutoService();
        private List<ItemPedido> _itensPedido = new List<ItemPedido>();
        private PedidoService _pedidoService = new PedidoService();

        public PedidoView(Pessoa pessoa)
        {
            try
            {
                InitializeComponent();
                _pessoa = pessoa;
                CarregarDadosPessoa();
                CarregarProdutos();
                CarregaHistoricoPedidos();
                GerarPedidoId();
            }
            catch (Exception ex) 
            { 
                MessageBox.Show($"Ocorreu um erro ao carregar os dados do pedido: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GerarPedidoId()
        {
            int novoId = _pedidoService.GerarNovoId();
            txtPedidoId.Text = novoId.ToString();
            txtDataVenda.Text = DateTime.Now.ToString("dd/MM/yyyy");
        }

        private void CarregarDadosPessoa()
        {
            txtClienteId.Text = _pessoa.Id.ToString();
            txtClienteNome.Text = _pessoa.Nome;
            txtClienteCpf.Text = _pessoa.CPF;
        }

        private void CarregarProdutos()
        {
            var produtos = _produtoService.GetProdutos();
            cbProdutos.ItemsSource = produtos.OrderBy(p => p.ProdutoDescricao).ToList();
            cbProdutos.DisplayMemberPath = "ProdutoDescricao";
            cbProdutos.SelectedValuePath = "Id";
        }

        private void btnAtualizarItem_Click(object sender, RoutedEventArgs e)
        {
            Button botao = (Button)sender;
            Pedido pedido = (Pedido)botao.DataContext;

            _pedidoService.AtualizarStatusPedido(pedido);
            MessageBox.Show("Status atualizado com sucesso!", "Informação", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnRemoverItem_Click(object sender, RoutedEventArgs e)
        {
            Button botaoRemove = (Button)sender;

            ItemPedido itemSelecionado = (ItemPedido)botaoRemove.DataContext;

            _itensPedido.Remove(itemSelecionado);
            dgItensPedido.ItemsSource = null;
            dgItensPedido.ItemsSource = _itensPedido;
            AtualizarValorTotal();

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
            try
            {
                if (cbProdutos.SelectedItem == null)
                {
                    MessageBox.Show(
                        "Selecione um produto.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);

                    return;
                }

                if (!int.TryParse(
                    txtQuantidade.Text,
                    out int quantidade))
                {
                    MessageBox.Show(
                        "Informe uma quantidade válida.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);

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
            catch (Exception ex)
            {
                MessageBox.Show($"Ocorreu um erro ao adicionar o produto ao pedido: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AtualizarValorTotal()
        {
            decimal total = _itensPedido.Sum(item => item.ValorTotal);
            txtValorTotal.Text = total.ToString("C");
        }

        private void btnFinalizarPedido_Click(object sender, RoutedEventArgs e)
        {
            if (_itensPedido.Count == 0)
            {
                MessageBox.Show("Adicione pelo menos um produto ao pedido.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cbFormaPagamento.SelectedItem == null)
            {
                MessageBox.Show("Selecione uma forma de pagamento.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBoxResult resultado = MessageBox.Show("Deseja realmente finalizar este pedido?", "Confirmar Pedido",MessageBoxButton.YesNo,MessageBoxImage.Question);

            if (resultado == MessageBoxResult.No)
            {
                return;
            }

            decimal total = _itensPedido.Sum(item => item.ValorTotal);

            ComboBoxItem formaPagamentoItem = (ComboBoxItem)cbFormaPagamento.SelectedItem;

            Pedido pedido = new Pedido
            {
                Id = int.Parse(txtPedidoId.Text),
                PessoaId = _pessoa.Id,
                NomePessoa = _pessoa.Nome,
                Itens = _itensPedido,
                ValorTotal = total,
                DataVenda = DateTime.Now,
                FormaPagamento = formaPagamentoItem.Content.ToString() ?? string.Empty,
                Status = "Pago"
            };

            _pedidoService.AdicionarPedido(pedido);
            MessageBox.Show("Pedido finalizado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

            CarregaHistoricoPedidos();
            LimparPedido();
        }

        private void LimparPedido()
        {
            _itensPedido.Clear();
            dgItensPedido.ItemsSource = null;
            txtValorTotal.Text = "R$ 0,00";
            cbFormaPagamento.SelectedIndex = -1;
            txtQuantidade.Clear();
            cbFormaPagamento.SelectedIndex = -1;
            GerarPedidoId();
        }

        private void CarregaHistoricoPedidos()
        {
            List<Pedido> pedidos = _pedidoService.GetPedidos().Where(p => p.PessoaId == _pessoa.Id).ToList();
            dgHistoricoPedidos.ItemsSource = null;
            dgHistoricoPedidos.ItemsSource = pedidos;
        }
    }
}
