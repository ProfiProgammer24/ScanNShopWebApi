using System.ComponentModel.DataAnnotations;

namespace ScanNShopWebApi.Models
{
    public class List
    {
        [Key]
        public int ListId { get; set; }

        public string Name { get; set; } // Beispielhafte Spalte
    }
}
