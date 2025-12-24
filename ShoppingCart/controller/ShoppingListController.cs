using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingCart.dto;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize]

public class ShoppingListController : ControllerBase
{
    private readonly IShoppingListService _shoppingListService;

    public ShoppingListController(IShoppingListService shoppingListService)
    {
        _shoppingListService = shoppingListService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ShoppingListDto>>> GetShoppingListById(int id)
    {
        try
        {
            var shoppingList = await _shoppingListService.GetShoppingListByIdAsync(id);
            return Ok(ApiResponse<ShoppingListDto>.Ok(shoppingList));
        }
        catch (BusinessException ex)
        {
            return NotFound(ApiResponse<ShoppingListDto>.Fail(ex.Code, ex.Message, 404));
        }
    }

    [HttpPost("create")]
    public async Task<ActionResult<ApiResponse<ShoppingListDto>>> Create([FromBody] CreateShoppingListDto createDto)
    {
        try
        {
            // 1. GÜVENLİ ID BULMA YÖNTEMİ 🕵️‍♂️
            // Önce standart yere bakar (nameid), bulamazsa "Id"ye, bulamazsa "id"ye bakar.
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) 
                            ?? User.FindFirst("Id") 
                            ?? User.FindFirst("id");

            if (userIdClaim == null) 
            {
                return Unauthorized(ApiResponse<string>.Fail("401", "Token içinde Kullanıcı ID bulunamadı. Lütfen tekrar giriş yapın.", 401));
            }
            
            int userId = int.Parse(userIdClaim.Value);

            // 2. Entity Hazırla
            var shoppingListEntity = new ShoppingList 
            { 
                Name = createDto.Name 
            };

            // 3. Servise Gönder
            var result = await _shoppingListService.CreateShoppingListAsync(shoppingListEntity, userId);

            return Ok(ApiResponse<ShoppingListDto>.Ok(result));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.Fail("400", ex.Message, 400));
        }
    }


    [HttpPut("update")]
    public async Task<ActionResult<ApiResponse<string>>> UpdateShoppingList([FromQuery] int id, [FromQuery] string name, [FromQuery] bool isCompleted)
    {
        try
        {
            // Service artık 3 parametre bekliyor: ID, İsim, Durum
            await _shoppingListService.UpdateShoppingListAsync(id, name, isCompleted);
            return Ok(ApiResponse<string>.Ok("Liste başarıyla güncellendi."));
        }
        catch (BusinessException ex)
        {
            return BadRequest(ApiResponse<string>.Fail(ex.Code, ex.Message, 400));
        }
    }
    
    [HttpDelete("delete/{id}")]
    public async Task<ActionResult<ApiResponse<string>>> DeleteShoppingList(int id)
    {
        try
        {
            await _shoppingListService.DeleteShoppingListAsync(id);
            return Ok(ApiResponse<string>.Ok("Shopping list deleted successfully"));
        }
        catch (BusinessException ex)
        {
            return NotFound(ApiResponse<string>.Fail(ex.Code, ex.Message, 404));
        }
    }

    [HttpPost("add-item")]
    public async Task<ActionResult<ApiResponse<string>>> AddItemToShoppingList([FromQuery] int shoppingListId, [FromQuery] int productId, [FromQuery] decimal quantity)
    {
        try
        {
            await _shoppingListService.AddItemToShoppingListAsync(shoppingListId, productId, quantity);
            return Ok(ApiResponse<string>.Ok("Item added to shopping list successfully"));
        }
        catch (BusinessException ex)
        {
            return BadRequest(ApiResponse<string>.Fail(ex.Code, ex.Message, 400));
        }
    }
    [HttpDelete("remove-item")]
    public async Task<ActionResult<ApiResponse<string>>> RemoveItemFromShoppingList([FromQuery] int shoppingListId, [FromQuery] int productId)
    {
        try
        {
            await _shoppingListService.RemoveItemFromShoppingListAsync(shoppingListId, productId);
            return Ok(ApiResponse<string>.Ok("Item removed from shopping list successfully"));
        }
        catch (BusinessException ex)
        {
            return BadRequest(ApiResponse<string>.Fail(ex.Code, ex.Message, 400));
        }
    }
    [HttpPut("update-item-quantity")]
    public async Task<ActionResult<ApiResponse<string>>> UpdateItemQuantity([FromQuery] int shoppingListId, [FromQuery] int productId, [FromQuery] decimal quantity)
    {
        try
        {
            await _shoppingListService.UpdateItemQuantityAsync(shoppingListId, productId, quantity);
            return Ok(ApiResponse<string>.Ok("Item quantity updated successfully"));
        }
        catch (BusinessException ex)
        {
            return BadRequest(ApiResponse<string>.Fail(ex.Code, ex.Message, 400));
        }
    }

    [HttpGet("my-shopping-lists")]
    public async Task<ActionResult<ApiResponse<List<ShoppingListDto>>>> GetMyShoppingLists()
    {
        var userId = GetCurrentUserId();
        var shoppingLists = await _shoppingListService.GetMyShoppingListsAsync(userId);
        return Ok(ApiResponse<List<ShoppingListDto>>.Ok(shoppingLists));
    }
    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim!);
    }

    [HttpPut("update-item-isChecked")]

    public async Task<ActionResult<ApiResponse<string>>> UpdateItemIsChecked([FromQuery] int shoppingListId, [FromQuery] int productId, [FromQuery] bool isChecked)
    {
        try
        {
            await _shoppingListService.UpdateItemIsCheckedAsync(shoppingListId, productId, isChecked);
            return Ok(ApiResponse<string>.Ok("Item isChecked status updated successfully"));
        }
        catch (BusinessException ex)
        {
            return BadRequest(ApiResponse<string>.Fail(ex.Code, ex.Message, 400));
        }
    }

    [HttpPost("add-member")]
    public async Task<ActionResult<ApiResponse<string>>> AddMemberToList([FromQuery] int shoppingListId, [FromQuery] int userId)
    {
        try
        {
            // 1. İşlemi yapan kişinin ID'sini token'dan al
            var currentUserId = GetCurrentUserId(); 

            // 2. Servise 3. parametre olarak bunu gönder
            await _shoppingListService.AddMemberToShoppingListAsync(shoppingListId, userId, currentUserId);
            
            return Ok(ApiResponse<string>.Ok("User added to the shopping list successfully."));
        }
        catch (BusinessException ex)
        {
            return BadRequest(ApiResponse<string>.Fail(ex.Code, ex.Message, 400));
        }
    }
    [HttpDelete("leave-list")]

    public async Task<ActionResult<ApiResponse<string>>> LeaveShoppingList([FromQuery] int shoppingListId)
    {
        try
        {
            var currentUserId = GetCurrentUserId(); 

            await _shoppingListService.LeaveShoppingListAsync(shoppingListId, currentUserId);
            
            return Ok(ApiResponse<string>.Ok("You have left the shopping list successfully."));
        }
        catch (BusinessException ex)
        {
            return BadRequest(ApiResponse<string>.Fail(ex.Code, ex.Message, 400));
        }
    }
}