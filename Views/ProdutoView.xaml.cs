using challange_by_coodesh.Models;
using challange_by_coodesh.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Lógica interna para ProdutoView.xaml
    /// </summary>
    public partial class ProdutoView : Window
    {
        private readonly ProdutoService _produtoService;
        private ObservableCollection<Produto> _produtos;
        private Produto? _produtoSelecionado;

        public ProdutoView()
        {
            InitializeComponent();

            _produtoService = new ProdutoService();
            var produtos = _produtoService.GetProdutos();
            _produtos = new ObservableCollection<Produto>(produtos);
            dgProdutos.ItemsSource = _produtos;
        }

        private void txtIncluir_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNome.Text) ||
                string.IsNullOrWhiteSpace(txtCodigoProduto.Text) ||
                string.IsNullOrWhiteSpace(txtValor.Text))
            {
                MessageBox.Show("Por favor, preencha todos os campos.");
                return;
            }

            decimal valor;

            if (!decimal.TryParse(txtValor.Text, out valor))
            {
                MessageBox.Show("Valor deve ser um número válido.");
                return;
            }

            int proximoId = _produtos.Any() ? _produtos.Max(p => p.Id) + 1 : 1;

            var produto = new Produto
            {
                Id = proximoId,
                Nome = txtNome.Text,
                CodigoProduto = txtCodigoProduto.Text,
                Valor = valor
            };

            _produtos.Add(produto);
            _produtoService.SaveProdutos(_produtos.ToList());
            LimparCampos();

        }

        private void LimparCampos()
        {
            txtId.Clear();
            txtNome.Clear();
            txtCodigoProduto.Clear();
            txtValor.Clear();
        }

        private void txtValor_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.,]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void dgProdutos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgProdutos.SelectedItem is Produto produto)
            {
                _produtoSelecionado = produto;
                txtId.Text = produto.Id.ToString();
                txtNome.Text = produto.Nome;
                txtCodigoProduto.Text = produto.CodigoProduto;
                txtValor.Text = produto.Valor.ToString("F2");
                BtnIncluir.IsEnabled = false;
                BloquearCampos();
            }
        }

        private void txtEditarProduto_Click(object sender, RoutedEventArgs e)
        {
            if (_produtoSelecionado == null)
            {
                MessageBox.Show("Selecione um produto para editar.");
                return;
            }

            HabilitarCampos();

            BtnIncluir.IsEnabled = false;
        }

        private void txtSalvarProduto_Click(object sender, RoutedEventArgs e)
        {
            if (_produtoSelecionado == null)
            {
                MessageBox.Show("Selecione um produto para salvar.");
                return;
            }

            decimal valor;

            if (!decimal.TryParse(txtValor.Text, out valor))
            {
                MessageBox.Show("Valor deve ser um número válido.");
                return;
            }

            _produtoSelecionado.Nome = txtNome.Text;
            _produtoSelecionado.CodigoProduto = txtCodigoProduto.Text;
            _produtoSelecionado.Valor = valor;
            dgProdutos.Items.Refresh();
            _produtoService.SaveProdutos(_produtos.ToList());
            BtnIncluir.IsEnabled = true;
            MessageBox.Show("Produto atualizado com sucesso!");
            LimparCampos();
        }

        private void BloquearCampos()
        {
            txtNome.IsEnabled = false;
            txtCodigoProduto.IsEnabled = false;
            txtValor.IsEnabled = false;
        }

        private void HabilitarCampos()
        {
            txtNome.IsEnabled = true;
            txtCodigoProduto.IsEnabled = true;
            txtValor.IsEnabled = true;
        }

        private void txtExcluirProduto_Click(object sender, RoutedEventArgs e)
        {
            if (_produtoSelecionado == null)
            {
                MessageBox.Show("Selecione um produto para excluir.");
                return;
            }

            var resultado = MessageBox.Show("Tem certeza que deseja excluir este produto?", "Confirmação", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (resultado != MessageBoxResult.Yes) 
            {
                return;
            }

            _produtos.Remove(_produtoSelecionado);
            _produtoService.SaveProdutos(_produtos.ToList());
            dgProdutos.Items.Refresh();
            LimparCampos();
            HabilitarCampos();
            BtnIncluir.IsEnabled = true;
            _produtoSelecionado = null;
            MessageBox.Show("Produto excluído com sucesso!");
        }

        private void BtnPesquisarProduto_Click(object sender, RoutedEventArgs e)
        {
            var produtos = _produtoService.GetProdutos();
            var query = produtos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(txtFiltrarNomeProduto.Text))
            {
                query = query.Where(p => p.Nome.Contains(txtFiltrarNomeProduto.Text, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(txtFiltrarCodigoProduto.Text))
            {
                query = query.Where(p => p.CodigoProduto.Contains(txtFiltrarCodigoProduto.Text, StringComparison.OrdinalIgnoreCase));
            }

            bool valorInicialPreenchido = !string.IsNullOrWhiteSpace(txtFiltrarValorInicial.Text);
            bool valorFinalPreenchido = !string.IsNullOrWhiteSpace(txtFiltrarValorFinal.Text);

            if (valorInicialPreenchido && !valorFinalPreenchido)
            {
                MessageBox.Show("Preencha o valor final para filtrar por valor.", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtFiltrarValorFinal.Focus();
                return;
            }

            if (valorInicialPreenchido && valorFinalPreenchido)
            {
                decimal valorInicial;
                decimal valorFinal;

                bool valorInicialValido = decimal.TryParse(txtFiltrarValorInicial.Text, out valorInicial);
                bool valorFinalValido = decimal.TryParse(txtFiltrarValorFinal.Text, out valorFinal);

                if (!valorInicialValido)
                {
                    MessageBox.Show("Valor inicial deve ser um número válido.", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtFiltrarValorInicial.Clear();
                    txtFiltrarValorInicial.Focus();
                    return;
                }

                if (!valorFinalValido)
                {
                    MessageBox.Show("Valor final deve ser um número válido.", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtFiltrarValorFinal.Clear();
                    txtFiltrarValorFinal.Focus();
                    return;
                }

                if (valorFinal < valorInicial) 
                {
                    MessageBox.Show("Valor final deve ser maior ou igual ao valor inicial.", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtFiltrarValorFinal.Clear();
                    txtFiltrarValorFinal.Focus();
                    return;
                }

                query = query.Where(p => p.Valor >= valorInicial && p.Valor <= valorFinal);

            }
                dgProdutos.ItemsSource = new ObservableCollection<Produto>(query.ToList());
        }
    }
}
