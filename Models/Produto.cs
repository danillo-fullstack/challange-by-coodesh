using System;
using System.Collections.Generic;
using System.Text;

namespace challange_by_coodesh.Models
{
    class Produto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string CodigoProduto { get; set; } = string.Empty;
        public decimal Valor { get; set; }
    }
}
