namespace ScanNShopWebApi.DTO
{
    public class ListWithProductsDto
    {
        public string Name { get; set; }
        public int? UserId { get; set; }
        public List<ProductDto> Products { get; set; }
    }

}
