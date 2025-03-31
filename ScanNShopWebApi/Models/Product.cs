namespace ScanNShopWebApi.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public int? ListId { get; set; }
        public string Name { get; set; }
        public int Quantitiy { get; set; }
        public bool isChecked { get; set; }

        // Falls du eine Navigation Property für die Liste willst:
        public List List { get; set; }
    }
}
