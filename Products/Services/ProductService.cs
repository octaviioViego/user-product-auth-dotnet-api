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

        
        SentenciaProductos crearSentencia = new SentenciaProductos(productsQueryDTO.Page,
                                                                   productsQueryDTO.Limit,
                                                                   productsQueryDTO.Sort,
                                                                   productsQueryDTO.Order,
                                                                   productsQueryDTO.Status,
                                                                   productsQueryDTO.isDeleted,
                                                                   productsQueryDTO.Type);
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
        /// Lanza un ok 200 y regresa una respuesta de tipo ResponseGetProducts que a su ves regresa los productos.
        /// </summary>    
        // return new OkObjectResult(new ApiResponse<ResponseGetProducts<List<ProductDTO>>>(200,MessageService.Instance.GetMessage("Productos200"), 
        // new ResponseGetProducts<List<ProductDTO>>(numResult, sentencia.valores[6], sentencia.valores[5], result, sentencia.valores[3], sentencia.valores[4], sentencia.valores[0], sentencia.valores[1], sentencia.valores[2])));
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
        
        // Actualizar solo los campos proporcionados en el DTO, preservando los que no se proporcionan
        product.type = productUpdateDTO.type;  
        product.name = productUpdateDTO.name;  
        product.price = productUpdateDTO.price; 
        product.status = productUpdateDTO.status; 

        // Los campos opcionales solo se actualizan si el cliente proporciona un valor
        product.description = productUpdateDTO.text ?? product.description; 
        product.image_link = productUpdateDTO.image_link ?? product.image_link;

        product.modified_at = DateTimeOffset.UtcNow; // Actualizamos la fecha de modificación

        // Guardar el producto actualizado
        await _iProductDAO.UpdateAsync(product);

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

        // Guardar los cambios en la base de datos
        await _iProductDAO.UpdateAsync(product);

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
