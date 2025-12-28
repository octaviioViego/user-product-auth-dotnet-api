using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

[Route("api/product")]
[Authorize(AuthenticationSchemes = "Bearer")]
[ApiController]
public class ProductController : ControllerBase , IProductsController
{
    private readonly IProductService _iProductService;

    public ProductController(IProductService productService)
    {
        _iProductService = productService;
    }

    /// <summary>
    /// Obtiene los productos de la base de datos con parametros opcionales.
    /// </summary>
    [HttpGet("product")]
    public async Task<IActionResult> GetAll2([FromQuery] ProductsQueryDTO productsQueryDTO)
    {
       try{
            var result = await _iProductService.GetAllProductsAsync(productsQueryDTO);
            return  Ok(new ApiResponse<ResponseGetProducts<List<ProductDTO>>>(200,MessageService.Instance.GetMessage("Productos200"), result));
       
       }catch (BusinessException ex){
             return StatusCode(ex.StatusCode,new ApiResponse<string>(ex.StatusCode, ex.Message));
            
       }catch(Exception ex){
             Console.WriteLine("Error 500: " + ex);
             return StatusCode(500,new ApiResponse<string>(500,MessageService.Instance.GetMessage("controllerGetAll500")));
       }
    }

    /// <summary>
    /// Recuperar un producto por id.
    /// </summary>
    [HttpGet("products/{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        try{

            var result = await _iProductService.GetProductByIdAsync(id);  
            return Ok(new ApiResponse<ProductDTO>(200,"Producto encontrado",result)); 

        }catch (BusinessException ex){
             return StatusCode(ex.StatusCode,new ApiResponse<string>(ex.StatusCode, ex.Message));
            
        }catch(Exception ex){
             Console.WriteLine("Error 500: " + ex);
             return StatusCode(500,new ApiResponse<string>(500,MessageService.Instance.GetMessage("controllerGetAll500")));
        }
    }

    /// <summary>
    /// Crear un nuevo producto.
    /// </summary>
   [HttpPost("products")]
    public async Task<IActionResult> Post([FromBody] ProductCreateDTO productDTO)
    {
        try{

            var result = await _iProductService.CreateProductAsync(productDTO);
            return Ok(new ApiResponse<ProductoResponseDTO>(200,MessageService.Instance.GetMessage("Productoscreate200"),result));  
        }catch (BusinessException ex){
             return StatusCode(ex.StatusCode,new ApiResponse<string>(ex.StatusCode, ex.Message));
            
        }catch(Exception ex){
             Console.WriteLine("Error 500: " + ex);
             return StatusCode(500,new ApiResponse<string>(500,MessageService.Instance.GetMessage("controllerGetAll500")));
        }
    }

    /// <summary>
    /// Actualizar un producto.
    /// </summary>
    [HttpPut("products/{id}")]
    public async Task<IActionResult> Put(Guid id, [FromBody] ProductUpdateDTO productUpdateDTO)
    {
        try{
        
            var result = await _iProductService.Put(id, productUpdateDTO);
            return Ok(new ApiResponse<string>(200,MessageService.Instance.GetMessage(result))); 
 
        }catch (BusinessException ex){
             return StatusCode(ex.StatusCode,new ApiResponse<string>(ex.StatusCode, ex.Message));
            
        }catch(Exception ex){
             Console.WriteLine("Error 500: " + ex);
             return StatusCode(500,new ApiResponse<string>(500,MessageService.Instance.GetMessage("controllerGetAll500")));
        }
    }

    /// <summary>
    /// Actualizar un producto parcialmente.
    /// </summary>
    [HttpPatch("products/{id}")]
    public async Task<IActionResult> Patch(Guid id, [FromBody] ProductPartialUpdateDTO productPartialUpdate)
    {
        try{

            var result = await _iProductService.Patch(id, productPartialUpdate);
            return Ok(new ApiResponse<string>(200, MessageService.Instance.GetMessage(result)));
   
        }catch (BusinessException ex){
             return StatusCode(ex.StatusCode,new ApiResponse<string>(ex.StatusCode, ex.Message));
            
        }catch(Exception ex){
                Console.WriteLine("Error 500: " + ex);
                return StatusCode(500,new ApiResponse<string>(500,MessageService.Instance.GetMessage("controllerGetAll500")));
        }
    }

    /// <summary>
    /// Eliminado logico.
    /// </summary>
    [HttpDelete("products/{id}")]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        try{
        
            var result = await _iProductService.DeleteProduct(id);
            return Ok(new ApiResponse<string>(200,MessageService.Instance.GetMessage(result))); 

        }catch (BusinessException ex){
             return StatusCode(ex.StatusCode,new ApiResponse<string>(ex.StatusCode, ex.Message));
            
        }catch(Exception ex){
                Console.WriteLine("Error 500: " + ex);
                return StatusCode(500,new ApiResponse<string>(500,MessageService.Instance.GetMessage("controllerGetAll500")));
        }
        
    }

    /// <summary>
    /// Recuperacion de productos eliminados.
    /// </summary>
     [HttpPatch("products/{id}/restore")]
    public async Task<IActionResult> RestoreProduct(Guid id)
    {   
        try
        {

            var result = await _iProductService.RestoreProduct(id);
            return Ok(new ApiResponse<string>(200,MessageService.Instance.GetMessage(result))); 
 
        }catch (BusinessException ex){
             return StatusCode(ex.StatusCode,new ApiResponse<string>(ex.StatusCode, ex.Message));
            
        }catch(Exception ex){
                Console.WriteLine("Error 500: " + ex);
                return StatusCode(500,new ApiResponse<string>(500,MessageService.Instance.GetMessage("controllerGetAll500")));
        }
    }  

    /// <summary>
    //  buscar por nombre o descripcion.
    /// </summary>
    [HttpGet("products/search/{search}")]
    public async Task<IActionResult> GetProductsSearch(string search)
    {
        try{
        
            var result = await _iProductService.GetProductsSearh(search);
            return Ok(new ApiResponse<List<ProductDTO>>(200,MessageService.Instance.GetMessage("ProductsSearh200"),result));  
        }catch (BusinessException ex){
             return StatusCode(ex.StatusCode,new ApiResponse<string>(ex.StatusCode, ex.Message));
            
        }catch(Exception ex){
                Console.WriteLine("Error 500: " + ex);
                return StatusCode(500,new ApiResponse<string>(500,MessageService.Instance.GetMessage("controllerGetAll500")));
        }
    }

    /// <summary>
    //  Listar productos por rango de precios.
    /// </summary>
    [HttpGet("{min_price}/{max_price}")]
    public async Task<IActionResult> GetProductsPrice(decimal min_price,decimal max_price)
    {
        try{
            
            var result = await _iProductService.GetProductsPrice(min_price,max_price);
            return Ok(new ApiResponse<List<ProductDTO>>(200,MessageService.Instance.GetMessage("ProductsPrice200"),result));
    

        }catch(Exception ex){
            Console.WriteLine("Error 500: " + ex);
            return StatusCode(500, new ApiResponse<string>(500,MessageService.Instance.GetMessage("controllerGetProductsPrice500")));
        }
        
    }

    /// <summary>
    //  Actualizar imagen.
    /// </summary> 
    [HttpPatch("products/{id}/update-image")]
    public async Task<IActionResult> UpdateImage(Guid id, [FromBody] ProductUpdateImageDTO updateImage)
    {   
        try{
           
            var result = await _iProductService.UpdateImage(id, updateImage);
            return Ok(new ApiResponse<string>(200,MessageService.Instance.GetMessage(result)));  

        }catch (BusinessException ex){
             return StatusCode(ex.StatusCode,new ApiResponse<string>(ex.StatusCode, ex.Message));
            
        }catch(Exception ex){
                Console.WriteLine("Error 500: " + ex);
                return StatusCode(500,new ApiResponse<string>(500,MessageService.Instance.GetMessage("controllerGetAll500")));
        }
    }

    /// <summary>
    //  Desactivar un producto.
    /// </summary> 
    [HttpPatch("products/{id}/deactivate")]
    public async Task<IActionResult> DeactivateProduct(Guid id)
    {
        try{
            
            var result = await _iProductService.DeactivateProduct(id);
            return Ok(new ApiResponse<string>(200,MessageService.Instance.GetMessage(result)));  
 
        
        }catch (BusinessException ex){
             return StatusCode(ex.StatusCode,new ApiResponse<string>(ex.StatusCode, ex.Message));
            
        }catch(Exception ex){
                Console.WriteLine("Error 500: " + ex);
                return StatusCode(500,new ApiResponse<string>(500,MessageService.Instance.GetMessage("controllerGetAll500")));
        }
        

    }

    /// <summary>
    //  Activar un producto.
    /// </summary> 
    [HttpPatch("products/{id}/activate")]
    public async Task<IActionResult> ActivateProduct(Guid id)
    {
        try{        
            
            var result = await _iProductService.ActivateProduct(id);
            return Ok(new ApiResponse<string>(200,MessageService.Instance.GetMessage(result)));  
  

        }catch (BusinessException ex){
             return StatusCode(ex.StatusCode,new ApiResponse<string>(ex.StatusCode, ex.Message));
            
        }catch(Exception ex){
                Console.WriteLine("Error 500: " + ex);
                return StatusCode(500,new ApiResponse<string>(500,MessageService.Instance.GetMessage("controllerGetAll500")));
        }
    }
}
