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
        private readonly string _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Data", "Pessoa.json");

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
            cpf = cpf.Replace(".", "").Replace("-", "").Trim();

            if (cpf.Length != 11 || !long.TryParse(cpf, out _))
            {
                return false;
            }

            var multiplicadores1 = new int[] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            var multiplicadores2 = new int[] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            var tempCpf = cpf.Substring(0, 9);
            var soma = 0;
            
            for (int i = 0; i < multiplicadores1.Length; i++)
            {
                soma += int.Parse(tempCpf[i].ToString()) * multiplicadores1[i];
            }
            
            var resto = soma % 11;
            var digito1 = resto < 2 ? "0" : (11 - resto).ToString();
            tempCpf += digito1;
            soma = 0;
            
            for (int i = 0; i < multiplicadores2.Length; i++)
            {
                soma += int.Parse(tempCpf[i].ToString()) * multiplicadores2[i];
            }
            resto = soma % 11;
            var digito2 = resto < 2 ? "0" : (11 - resto).ToString();

            return cpf.EndsWith(digito1 + digito2);
        }
    }
}
