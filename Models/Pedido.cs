using System;
using System.Collections.Generic;
using System.Text;

namespace challange_by_coodesh.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        public int PessoaId { get; set; }
        public string NomePessoa { get; set; }
        public List<PedidoItem> Itens { get; set; }
        public decimal ValorTotal { get; set; }
        public DateTime DataVenda { get; set; }
        public string FormaPagamento { get; set; }
        public string Status { get; set; }

        public string Situacao { get; set; }

        public Pedido()
        {
            Itens = new List<PedidoItem>();
            Status = "Pendente";
            DataVenda = DateTime.Now;
        }

    }
}
