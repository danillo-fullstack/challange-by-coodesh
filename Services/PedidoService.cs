using challange_by_coodesh.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;

namespace challange_by_coodesh.Services
{
    public class PedidoService
    {
        private readonly string _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Pedido.json");

        public List<Pedido> GetPedidos()
        {
            if (!File.Exists(_filePath))
            {
                File.WriteAllText(_filePath, "[]");
            }

            var json = File.ReadAllText(_filePath);

            var pedidos = JsonSerializer.Deserialize<List<Pedido>>(json);

            return pedidos ?? new List<Pedido>();
        }

        public void SavePedidos(List<Pedido> pedidos)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(pedidos, options);

            File.WriteAllText(_filePath, json);
        }

    }
}
