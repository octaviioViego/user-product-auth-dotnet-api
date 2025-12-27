using productos.Methods;

public class ProductService : IProductService
{
    private readonly IProductDAO _iProductDAO;
    private readonly ProductMapper _productMapper;
    private readonly CreateProductMapper _createProductoMapper;

    public ProductService(IProductDAO iProductDAO, ProductMapper productMapper, CreateProductMapper createProductoMapper)
    {
        _iProductDAO = iProductDAO;
        _productMapper = productMapper;
        _createProductoMapper = createProductoMapper;
    }

    /// <summary>
    /// Obtiene todos los productos de la base de datos.
    /// </summary>
    public async Task<ResponseGetProducts<List<ProductDTO>>> GetAllProductsAsync(ProductsQueryDTO productsQueryDTO)
    {
    
        if(!productsQueryDTO.Page.HasValue 
            && !productsQueryDTO.Limit.HasValue 
            && string.IsNullOrEmpty(productsQueryDTO.Sort) 
            && string.IsNullOrEmpty(productsQueryDTO.Order)
            && !productsQueryDTO.Status.HasValue
            && !productsQueryDTO.isDeleted.HasValue
            && string.IsNullOrEmpty(productsQueryDTO.Type))
        {
            productsQueryDTO.isDeleted = false;
        }

        SentenciaProductos crearSentencia = new SentenciaProductos(productsQueryDTO);
        var sentencia = crearSentencia.CrearSenentiaSQLProduct();

        var products = await _iProductDAO.GetProducts(sentencia.Sentencia,sentencia.Parametros);  
        
        if (products == null || !products.Any())
        {
            throw new BusinessException(404, MessageService.Instance.GetMessage("Productos404"));  
        }
        
        //Contamos el numero de resultados
        var numResult = products.Count();
        
        // Mapear los productos a DTO antes de devolverlos
        var result = products.Select(p => _productMapper.ToDTO(p)).ToList();
        
        /// <summary>
        /// Respuesta de tipo ResponseGetProducts que a su ves regresa los productos.
        /// </summary>    
        return new ResponseGetProducts<List<ProductDTO>>(numResult, 
                                                         sentencia.valores[6], 
                                                         sentencia.valores[5], 
                                                         result, 
                                                         sentencia.valores[3], 
                                                         sentencia.valores[4], 
                                                         sentencia.valores[0], 
                                                         sentencia.valores[1], 
                                                         sentencia.valores[2]);

    }

    /// <summary>
    /// Recuperar un producto por id.
    /// </summary>
    public async Task<ProductDTO> GetProductByIdAsync(Guid id)
    {
        var product = await _iProductDAO.GetByIdAsync(id); 
        if (product == null)
        {
            throw new BusinessException(404, MessageService.Instance.GetMessage("Productos404"));  
        }


        // Mapear el producto a DTO antes de devolverlo
        var result = _productMapper.ToDTO(product);
        return result;
    }

    /// <summary>
    /// Crear un nuevo producto.
    /// </summary>
    public async Task<ProductoResponseDTO> CreateProductAsync(ProductCreateDTO productDTO)
    {
        
        if (productDTO.price <= 0)
        {
             throw new BusinessException(400, MessageService.Instance.GetMessage("controllerProductDTO400"));  
        }

        // Crear entidad usando el Mapper
        var product = _createProductoMapper.ToEntity(productDTO);

        await _iProductDAO.AddAsync(product);
    
        var result = _productMapper.ToDTO(product);

        var respuestaDTO = _productMapper.productoResponseDTO(result);
        
        return respuestaDTO;
    }


    /// <summary>
    /// Actualizar un producto.
    /// </summary>
    public async Task<string> Put(Guid id, ProductUpdateDTO productUpdateDTO)
    {
        var product = await _iProductDAO.GetByIdAsync(id);
        if (product == null)
        {
             throw new BusinessException(404, MessageService.Instance.GetMessage("ProductUpdate404"));  
        }
        
        if(productUpdateDTO.price <= 0)  
        {
            throw new BusinessException(400, MessageService.Instance.GetMessage("ProductUpdate400"));  
        }
        
        var prodcutoPut = _productMapper.toPut(product,productUpdateDTO);
        // Guardar el producto actualizado
        await _iProductDAO.UpdateAsync(prodcutoPut);

        return "ProductUpdate200";
    }


    /// <summary>
    /// Actualizar un producto parcialmente.
    /// </summary>
    public async Task<string> Patch(Guid id, ProductPartialUpdateDTO productPartialUpdate)
    {
        // Buscar el producto por ID en la base de datos
        var product = await _iProductDAO.GetByIdAsync(id);
        
        if (product == null)
        {
            throw new BusinessException(404, MessageService.Instance.GetMessage("ProductPartialUpdate404"));  
        }

        if(product.is_deleted)
        {
            throw new BusinessException(400, MessageService.Instance.GetMessage("ProductPartialUpdate400"));  
        }
        
        if(productPartialUpdate.price <= 0)
        {
            throw new BusinessException(400, MessageService.Instance.GetMessage("controllerProductDTO400"));  
        }

        var productPatch = _productMapper.toPacth(product,productPartialUpdate);
        // Guardar los cambios en la base de datos
        await _iProductDAO.UpdateAsync(productPatch);

        return "ProductPartialUpdate200";
    }


    /// <summary>
    /// Eliminado logico.
    /// </summary>
    public async Task<string> DeleteProduct(Guid id)
    {
        var product = await _iProductDAO.GetByIdAsync(id);
        if (product == null)
        {
            throw new BusinessException(404, MessageService.Instance.GetMessage("DeleteProduct404"));  
        }

        //Si el producto ya se encuantra eliminado no se puede volver a eliminar.
        if(product.is_deleted)
        {
            throw new BusinessException(400, MessageService.Instance.GetMessage("DeleteProduct400"));  
        }

        product.is_deleted = true;
        product.deleted_at = DateTimeOffset.UtcNow;

        await _iProductDAO.UpdateAsync(product);
        return "DeleteProduct200";
        
    }

    /// <summary>
    /// Recuperacion de productos eliminados.
    /// </summary>
    public async Task<string> RestoreProduct(Guid id)
    {
        var product = await _iProductDAO.GetByIdAsync(id);
        if (product == null)
        {
            throw new BusinessException(404, MessageService.Instance.GetMessage("RestoreProduct404"));  
        }
        //Si es producto no esta elimindo no se puede restaurar.
        if (!product.is_deleted)
        {
            throw new BusinessException(400, MessageService.Instance.GetMessage("RestoreProduct400"));  
        }

        product.is_deleted = false;
        product.deleted_at = null;

        await _iProductDAO.UpdateAsync(product);
        
        return "RestoreProduct200";
    }

    /// <summary>
    //  buscar por nombre o descripcion.
    /// </summary>
    public async Task<List<ProductDTO>> GetProductsSearh(string search){
        
        var products = await _iProductDAO.GetProductsSearh(search);
        
        if (products == null || products.Count == 0)
        {
            throw new BusinessException(404, MessageService.Instance.GetMessage("ProductsSearh404"));  
        }
        // Mapear los productos a DTO (si es necesario) antes de enviarlos en la respuesta
        
        var result = products.Select(p => _productMapper.ToDTO(p)).ToList();
        return result;
        
    }


    /// <summary>
    //  Listar productos por rango de precios.
    /// </summary>
    public async Task<List<ProductDTO>> GetProductsPrice(decimal min_price, decimal max_price)
    {
        
         if (min_price <= 0)
         {
            throw new BusinessException(400, MessageService.Instance.GetMessage("controllerProductsPrice400"));  
        }

         if (max_price < min_price)
         {
             throw new BusinessException(400, MessageService.Instance.GetMessage("controllerProductsPrice400"));  
        }

        var product = await _iProductDAO.GetProductsPrice(min_price,max_price);
        
        if(product == null || !product.Any())
        {
            throw new BusinessException(404, MessageService.Instance.GetMessage("ProductsPrice404"));  
        }

        var result = product.Select(p => _productMapper.ToDTO(p)).ToList();
        return result;
        
    }

    /// <summary>
    //  Actualizar imagen.
    /// </summary> 
    public async Task<string> UpdateImage(Guid id, ProductUpdateImageDTO updateImage){
        
        var existingProduct = await _iProductDAO.GetByIdAsync(id);
        if (existingProduct == null)
        {
            throw new BusinessException(404, MessageService.Instance.GetMessage("UpdateImage404"));  
        }

        existingProduct.image_link = updateImage.image_link;

        await _iProductDAO.UpdateAsync(existingProduct);
        return "UpdateImage200";
    }

    /// <summary>
    //  Desactivar un producto.
    /// </summary> 
    public async Task<string> DeactivateProduct(Guid id){

        var product = await _iProductDAO.GetByIdAsync(id);
        
        if (product == null)
        {
            throw new BusinessException(404, MessageService.Instance.GetMessage("DeactivateProduct404"));  
        }

        if (!product.status)
        {
            throw new BusinessException(400, MessageService.Instance.GetMessage("DeactivateProduct400"));  
        }

        // Realizamos la desactivación lógica
        product.status = false;

        await _iProductDAO.UpdateAsync(product);
        
        return "DeactivateProduct200";
    }

    /// <summary>
    //  Activar un producto.
    /// </summary> 
    public async Task<string> ActivateProduct(Guid id)
    {
        var product = await _iProductDAO.GetByIdAsync(id);
        if (product == null)
        {
            throw new BusinessException(404, MessageService.Instance.GetMessage("ActivateProduct404"));  
        }

        if (product.status)
        {
            throw new BusinessException(400, MessageService.Instance.GetMessage("ActivateProduct400"));  
        }

        // Activación lógica del producto
        product.status = true;
        await _iProductDAO.UpdateAsync(product);
        return "ActivateProduct200";
    }

}
