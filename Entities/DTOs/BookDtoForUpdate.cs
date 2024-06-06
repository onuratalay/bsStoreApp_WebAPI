using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public record BookDtoForUpdate(int Id, string Title, decimal Price)
    {
        //public int Id { get; init; } // init: readonly yani immutable özelliği olmasını sağlar.
        //public string Title { get; init; }
        //public decimal Price { get; init; }
    }
}