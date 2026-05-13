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

        private int _quantidade;

        public int Quantidade
        {
            get { return _quantidade; }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentException("A quantidade deve ser de pelo menos 1.");
                }
                _quantidade = value;
            }
        }
        public string Status { get; set; } = "Pendente";

        public decimal ValorTotal
        {
            get { return ValorUnitario * Quantidade; }
        }

        
    }
}
