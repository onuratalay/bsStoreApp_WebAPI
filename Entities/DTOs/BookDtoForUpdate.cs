using System.ComponentModel.DataAnnotations;

namespace Entities.DTOs
{
    public record BookDtoForUpdate : BookDtoForManipulation
    {
        [Required]
        public int Id { get; set; }

        //public int Id { get; init; } // init: readonly yani immutable özelliği olmasını sağlar.
        //public string Title { get; init; }
        //public decimal Price { get; init; }
    }
}