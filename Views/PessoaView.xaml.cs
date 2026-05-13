using challange_by_coodesh.Models;
using challange_by_coodesh.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Lógica interna para PessoaView.xaml
    /// </summary>
    public partial class PessoaView : Window
    {
        private readonly PessoaService _pessoaService;
        private ObservableCollection<Pessoa> _pessoas;

        public PessoaView()
        {
            InitializeComponent();
            _pessoaService = new PessoaService();
            var pessoas = _pessoaService.GetPessoas();
            _pessoas = new ObservableCollection<Pessoa>(pessoas);
            dgPessoa.ItemsSource = _pessoas;
        }

        private void btnIncluir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtNome.Text) || string.IsNullOrWhiteSpace(txtCPF.Text) || string.IsNullOrWhiteSpace(txtEndereco.Text))
                {
                    MessageBox.Show("Por favor, preencha todos os campos.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!_pessoaService.IsCpfValido(txtCPF.Text)) {
                    MessageBox.Show("CPF inválido. Por favor, insira um CPF válido.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                bool cpfExistente = _pessoas.Any(p => p.CPF == txtCPF.Text);
                if (cpfExistente)
                {
                    MessageBox.Show("CPF já cadastrado. Por favor, insira um CPF diferente.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int proximoId = _pessoas.Any() ? _pessoas.Max(p => p.Id) + 1 : 1;

                var pessoa = new Pessoa
                {
                    Id = proximoId,
                    Nome = txtNome.Text,
                    CPF = txtCPF.Text,
                    Endereco = txtEndereco.Text
                };

                txtId.Text = proximoId.ToString();

                _pessoas.Add(pessoa);
                _pessoaService.SavePessoas(_pessoas.ToList());

                LimparCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao incluir pessoa: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        private void LimparCampos()
        {
            txtId.Text = "";
            txtNome.Text = "";
            txtCPF.Text = "";
            txtEndereco.Text = "";

            txtNome.Focus();
        }

        private void txtCPF_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            string texto = new string(textBox.Text.Where(char.IsDigit).ToArray());

            if(texto.Length >=1)
            {
                if (texto.Length <= 3)
                {
                    textBox.Text = texto;

                }
                else if (texto.Length <= 6)
                {
                    textBox.Text = $"{texto.Substring(0, 3)}.{texto.Substring(3)}";
                }
                else if (texto.Length <= 9)
                {
                    textBox.Text = $"{texto.Substring(0, 3)}.{texto.Substring(3, 3)}.{texto.Substring(6)}";
                }
                else
                {
                    textBox.Text = $"{texto.Substring(0, 3)}.{texto.Substring(3, 3)}.{texto.Substring(6, 3)}-{texto.Substring(9, Math.Min(2, texto.Length - 9))}";
                }

                textBox.CaretIndex = textBox.Text.Length;
            }
        }

        private void dgPessoa_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgPessoa.SelectedItem is Pessoa pessoaSelecionada)
            {
                txtId.Text = pessoaSelecionada.Id.ToString();
                txtNome.Text = pessoaSelecionada.Nome;
                txtCPF.Text = pessoaSelecionada.CPF;
                txtEndereco .Text = pessoaSelecionada.Endereco;

                txtNome.IsEnabled = false;
                txtCPF.IsEnabled = false;
                txtEndereco.IsEnabled = false;

                btnEditar.IsEnabled = true;
                btnExcluir.IsEnabled = true;
                btnIncluir.IsEnabled = false;

            }
        }

        private void BtnSalvar_Click(object sender, RoutedEventArgs e)
        {
            string id = txtId.Text;

            var dadosAtualizados = _pessoas.FirstOrDefault(p => p.Id.ToString() == id);

            if (dadosAtualizados != null)
            {
                dadosAtualizados.Nome = txtNome.Text;
                dadosAtualizados.CPF = txtCPF.Text;
                dadosAtualizados.Endereco = txtEndereco.Text;
                _pessoaService.SavePessoas(_pessoas.ToList());
                dgPessoa.Items.Refresh();
                LimparCampos();
                MessageBox.Show("Dados atualizados com sucesso!", "Informação", MessageBoxButton.OK, MessageBoxImage.Information);
                btnIncluir.IsEnabled = true;
                HabilitarAcoes();
            } 
            else
            {
                MessageBox.Show("Pessoa não encontrada para atualização.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        private void HabilitarAcoes()
        {
            txtNome.IsEnabled = true;
            txtCPF.IsEnabled = true;
            txtEndereco.IsEnabled = true;

            dgPessoa.SelectedItem = null;
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (dgPessoa.SelectedItem != null)
            {
                txtNome.IsEnabled = true;
                txtCPF.IsEnabled = true;
                txtEndereco.IsEnabled = true;
                txtNome.Focus();

                btnSalvar.IsEnabled = true;
                btnIncluir.IsEnabled = false;
            }
            else
            {
                MessageBox.Show("Por favor, selecione uma pessoa para editar.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }

        private void btnExcluir_Click(object sender, RoutedEventArgs e)
        {
            if (dgPessoa.SelectedItem is Pessoa pessoaSelecionada)
            {
                var resultado = MessageBox.Show($"Tem certeza que deseja excluir {pessoaSelecionada.Nome}?", "Confirmação de Exclusão", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (resultado == MessageBoxResult.Yes)
                {
                    _pessoas.Remove(pessoaSelecionada);
                    _pessoaService.SavePessoas(_pessoas.ToList());
                    HabilitarAcoes();
                    btnIncluir.IsEnabled = true;
                    LimparCampos();
                    MessageBox.Show("Pessoa excluída com sucesso!", "Informação", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecione uma pessoa para excluir.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnPesquisar_Click(object sender, RoutedEventArgs e)
        {
            string filtroNome = txtFiltrarNome.Text.ToLower().Trim();
            string filtroCPF = txtFiltrarCPF.Text.Trim();

            if (string.IsNullOrWhiteSpace(filtroNome) && string.IsNullOrWhiteSpace(filtroCPF))
            {
                dgPessoa.ItemsSource = _pessoas;
                return;
            }

            var resultado = _pessoas.Where(p =>
            {
                bool correspondeNome = string.IsNullOrWhiteSpace(filtroNome) || p.Nome.ToLower().Contains(filtroNome);
                bool correspondeCPF = string.IsNullOrWhiteSpace(filtroCPF) || p.CPF.Contains(filtroCPF);
                return correspondeNome && correspondeCPF;
            }).ToList();

            dgPessoa.ItemsSource = resultado;

            if (resultado.Count == 0)
            {
                MessageBox.Show("Nenhuma pessoa encontrada com os critérios de pesquisa.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnIncluirPedido_Click(object sender, RoutedEventArgs e)
        {
            if (dgPessoa.SelectedItem is Pessoa pessoaSelecionada)
            {
                var pedidoView = new PedidoView(pessoaSelecionada);
                pedidoView.ShowDialog();
            }
            else
            {
                MessageBox.Show("Por favor, selecione uma pessoa para incluir um pedido.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
