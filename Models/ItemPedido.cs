using System;
using System.Collections.Generic;
using System.Text;

namespace challange_by_coodesh.Models
{
    public class ItemPedido
    {
        public int ProdutoId { get; set; }
        public string NomeProduto { get; set; }= string.Empty;
        public decimal ValorUnitario { get; set; }
        public int Quantidade { get; set; }
        public string Status { get; set; } = "Pendente";
        public string Situacao { get; set; } = "Processamento";

        public decimal ValorTotal
        {
            get { return ValorUnitario * Quantidade; }
        }

        
    }
}
