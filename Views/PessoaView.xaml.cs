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
                if (string.IsNullOrWhiteSpace(txtNome.Text) || string.IsNullOrWhiteSpace(txtCPF.Text))
                {
                    MessageBox.Show("Por favor, preencha todos os campos.");
                    return;
                }

                if (!_pessoaService.IsCpfValido(txtCPF.Text)) {
                    MessageBox.Show("CPF inválido. Por favor, insira um CPF válido.");
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
                MessageBox.Show($"Erro ao incluir pessoa: {ex.Message}");
            }
            
        }

        private void LimparCampos()
        {
            txtNome.Text = "";
            txtCPF.Text = "";
            txtEndereco.Text = "";
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

            }
        }

        private void btnSalvar_Click(object sender, RoutedEventArgs e)
        {
            string id = txtId.Text;

            if (!_pessoaService.IsCpfValido(txtCPF.Text))
            {
                MessageBox.Show("CPF inválido. Por favor, insira um CPF válido.");
                return;
            }

            var dadosAtualizados = _pessoas.FirstOrDefault(p => p.Id.ToString() == id);

            if (dadosAtualizados != null)
            {
                dadosAtualizados.Nome = txtNome.Text;
                dadosAtualizados.CPF = txtCPF.Text;
                dadosAtualizados.Endereco = txtEndereco.Text;
                _pessoaService.SavePessoas(_pessoas.ToList());
                dgPessoa.Items.Refresh();
                LimparCampos();
                MessageBox.Show("Dados atualizados com sucesso!");
            }
            else
            {
                MessageBox.Show("Pessoa não encontrada para atualização.");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (dgPessoa.SelectedItem != null)
            {
                txtNome.IsEnabled = true;
                txtCPF.IsEnabled = true;
                txtEndereco.IsEnabled = true;
                txtNome.Focus();
            }
            else
            {
                MessageBox.Show("Por favor, selecione uma pessoa para editar.");
            }
        }
    }
}
