using productos.Models;

public class ProductMapper
{
    /// <summary>
    /// Convierte un objeto de tipo Product a ProductDTO.
    /// </summary>
    /// <param name="product">El objeto Product que se desea convertir.</param>
    /// <returns>Un objeto ProductDTO con los datos del Product.</returns>
    public ProductDTO ToDTO(Product product)
    {
        return new ProductDTO
        {
            id = product.id,
            created_at = product.created_at,
            modified_at = product.modified_at,
            is_deleted = product.is_deleted,
            deleted_at = product.deleted_at,
            type = product.type,
            name = product.name,
            price = product.price,
            status = product.status,
            text = product.description,
            Product_key = product.product_key,
            image_link = product.image_link 
        };
    }

    /// <summary>
    /// Convierte un objeto de tipo ProductDTO a Product (entidad).
    /// </summary>
    /// <param name="product">El objeto ProductDTO que se desea convertir.</param>
    /// <returns>Un objeto Product con los datos del ProductDTO.</returns>
    public Product ToEntity(ProductDTO product)
    {
        return new Product
        {
            id = product.id,
            created_at = product.created_at,
            modified_at = product.modified_at,
            is_deleted = product.is_deleted,
            deleted_at = product.deleted_at,
            type = product.type,
            name = product.name,
            price = product.price,
            status = product.status,
            description = product.text,
            product_key = product.Product_key,
            image_link = product.image_link 
        };
    }

    public ProductoResponseDTO productoResponseDTO(ProductDTO productDTO)
    {
        return new ProductoResponseDTO
        {
            name = productDTO.name,
            type = productDTO.type,
            price = productDTO.price,
        };
    }

    public Product toPut(Product product, ProductUpdateDTO productUpdateDTO)
    {
        
            // Actualizar solo los campos proporcionados en el DTO, preservando los que no se proporcionan
            product.type = productUpdateDTO.type;  
            product.name = productUpdateDTO.name;  
            product.price = productUpdateDTO.price; 
            product.status = productUpdateDTO.status; 

            // Los campos opcionales solo se actualizan si el cliente proporciona un valor
            product.description = productUpdateDTO.text ?? product.description; 
            product.image_link = productUpdateDTO.image_link ?? product.image_link;

            product.modified_at = DateTimeOffset.UtcNow; // Actualizamos la fecha de modificación
            
            return product;
    }

    public Product toPacth(Product product, ProductPartialUpdateDTO productPartialUpdate)
    {
        // Actualización parcial: solo se modifican los valores que no sean null en el DTO
        product.type = productPartialUpdate.type ?? product.type;
        product.name = productPartialUpdate.name ?? product.name;
        product.price = productPartialUpdate.price ?? product.price;
        product.status = productPartialUpdate.status ?? product.status;
        product.description = productPartialUpdate.text ?? product.description;
        product.product_key = productPartialUpdate.Product_key ?? product.product_key;
        product.image_link = productPartialUpdate.image_lick ?? product.image_link; 

        // Registrar la última modificación
        product.modified_at = DateTimeOffset.UtcNow;
        return product;
    }
}
