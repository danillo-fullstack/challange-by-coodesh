using challange_by_coodesh.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace challange_by_coodesh.Services
{
    class PessoaService
    {
        private readonly string _filePath = "Data/Pessoa.json";

        public List<Pessoa> GetPessoas()
        {
            if (!File.Exists(_filePath))
            {
                return new List<Pessoa>();
            }
            var json = File.ReadAllText(_filePath);
            
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<Pessoa>();
            }

            return JsonSerializer.Deserialize<List<Pessoa>>(json) ?? new List<Pessoa>();
        }

        public void SavePessoas(List<Pessoa> pessoas)
        {
            var json = JsonSerializer.Serialize(pessoas, new JsonSerializerOptions { WriteIndented = true });
            string? directory = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(directory))
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }
            File.WriteAllText(_filePath, json);
        }

        public bool IsCpfValido(string cpf)
        {
            cpf = new string(cpf.Where(char.IsDigit).ToArray());

            if(cpf.Length != 11)
            {
                return false;
            }

            if (new string(cpf[0], 11) == cpf)
            {
                return false;
            }

            int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            string tempCpf = cpf.Substring(0, 9);
            int soma = 0;

            for (int i = 0; i < 9; i++)
            {
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];
            }

            int resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;

            string digito = resto.ToString();
            tempCpf += digito;
            soma = 0;

            for (int i = 0; i < 10; i++)
            {
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];
            }

            resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;
            digito = digito + resto.ToString();

            return cpf.EndsWith(digito);
        }
    }
}
