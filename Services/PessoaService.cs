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
    }
}
