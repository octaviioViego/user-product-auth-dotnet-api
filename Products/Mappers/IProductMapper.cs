using productos.Models;

public interface IProductMapper
{
    public ProductDTO ToDTO(Product product);
    public Product ToEntity(ProductDTO product);
    public ProductoResponseDTO productoResponseDTO(ProductDTO productDTO);
    public Product toPut(Product product, ProductUpdateDTO productUpdateDTO);
    public Product toPacth(Product product, ProductPartialUpdateDTO productPartialUpdate);
    public Product CreateProductMapper(ProductCreateDTO productDTO);
}