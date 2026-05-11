using challange_by_coodesh.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace challange_by_coodesh.Services
{
    class ProdutoService
    {
        private readonly string _filePath = "Data/Produto.json";

        public List<Produto> GetProdutos()
        {
            if (!File.Exists(_filePath))
            {
                return new List<Produto>();
            }
            
            var json = File.ReadAllText(_filePath);

            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<Produto>();
            }

            return JsonSerializer.Deserialize<List<Produto>>(json) ?? new List<Produto>();
        }

        public void SaveProdutos(List<Produto> produtos)
        {
            var json = JsonSerializer.Serialize(produtos, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
    }
}
