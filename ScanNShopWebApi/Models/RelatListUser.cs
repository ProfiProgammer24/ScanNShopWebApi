using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ScanNShopWebApi.Models
{
    [Table("Relat_List_User")]
    public class RelatListUser
{
    public int Relat_UserId { get; set; }
    public int Relat_ListId { get; set; }

    [ForeignKey("Relat_ListId")]
    public List? List { get; set; }

    [ForeignKey("Relat_UserId")]
    public User? User { get; set; }
}
}
