using System;
using System.Collections.Generic;
using System.Text;

namespace challange_by_coodesh.Models
{
    public class PedidoItem
    {
        public int ProdutoId { get; set; }
        public string NomeProduto { get; set; }
        public decimal ValorUnitario { get; set; }
        public int Quantidade { get; set; }

        public decimal ValorTotal
        {
            get { return ValorUnitario * Quantidade; }
        }
    }
}
