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
    }
}
