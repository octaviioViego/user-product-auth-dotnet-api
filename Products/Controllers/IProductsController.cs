using Microsoft.AspNetCore.Mvc;

public interface IProductsController
{
    Task<IActionResult> GetAll2(ProductsQueryDTO productsQueryDTO);
    Task<IActionResult> Get(Guid id);
    Task<IActionResult> Post([FromBody] ProductCreateDTO productDTO);
    Task<IActionResult> Put(Guid id, [FromBody] ProductUpdateDTO productUpdateDTO);
    Task<IActionResult> Patch(Guid id, [FromBody] ProductPartialUpdateDTO productPartialUpdate);
    Task<IActionResult> DeleteProduct(Guid id);
    Task<IActionResult> RestoreProduct(Guid id);
    Task<IActionResult> GetProductsSearch(string search);
    Task<IActionResult> GetProductsPrice(decimal min_price,decimal max_price);
    Task<IActionResult> UpdateImage(Guid id, [FromBody] ProductUpdateImageDTO updateImage);
    Task<IActionResult> DeactivateProduct(Guid id);
    Task<IActionResult> ActivateProduct(Guid id);
    
}