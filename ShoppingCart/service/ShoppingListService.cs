using System.Security.Cryptography.X509Certificates;

public class ShoppingListService : IShoppingListService
{
    private readonly IShoppingListRepository _shoppingListRepository;

    private readonly IProductRepository _productRepository;

    private readonly IConnectionRepository _connectionRepository;

    public ShoppingListService(IShoppingListRepository shoppingListRepository, IProductRepository productRepository, IConnectionRepository connectionRepository)
    {
        _shoppingListRepository = shoppingListRepository;
        _productRepository = productRepository;
        _connectionRepository = connectionRepository;
    }

    public async Task<ShoppingListDto> GetShoppingListByIdAsync(int id)
    {
        var shoppingList = await _shoppingListRepository.GetByIdAsync(id);
        if(shoppingList == null){
            throw new BusinessException("SHOPPING_LIST_NOT_FOUND", "Shopping list not found");
        }
        return new ShoppingListDto
        {
            Id = shoppingList.Id,
            Name = shoppingList.Name,
            CreatedAt = shoppingList.CreatedAt,
            IsCompleted = shoppingList.IsCompleted,
            Items = shoppingList.Items.Select(item => new ShoppingListProductDto
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                IsChecked = item.IsChecked
            }).ToList()
        }; 
    }

    // ShoppingListService.cs

public async Task<ShoppingListDto> CreateShoppingListAsync(ShoppingList shoppingList, int userId)
{
    // 1. Listenin yaratılma tarihi ve diğer varsayılanlarını ayarla
    shoppingList.CreatedAt = DateTime.UtcNow;
    shoppingList.IsCompleted = false;

    // 2. KRİTİK NOKTA: Listeyi oluşturan kullanıcıyı "Üye" olarak ekle!
    // Eğer bunu yapmazsan liste sahipsiz kalır ve MyList'te görünmez.
    if (shoppingList.Members == null) 
    {
        shoppingList.Members = new List<ShoppingListMember>();
    }

    shoppingList.Members.Add(new ShoppingListMember 
    {
        UserId = userId,
        // Eğer Membership tablosunda JoinedAt varsa: JoinedAt = DateTime.UtcNow
    });

    // 3. Veritabanına kaydet (Repository, içindeki Members ile birlikte kaydeder)
    var createdShoppingList = await _shoppingListRepository.CreateAsync(shoppingList);

    // 4. DTO'ya çevirip döndür
    return new ShoppingListDto
    {
        Id = createdShoppingList.Id,
        Name = createdShoppingList.Name,
        CreatedAt = createdShoppingList.CreatedAt,
        IsCompleted = createdShoppingList.IsCompleted, // DTO'na bunu eklemeyi unutma
        Items = new List<ShoppingListProductDto>() // Yeni liste boştur
    };
}

    public async Task UpdateShoppingListAsync(int id, string name, bool isCompleted)
{
    var shoppingList = await _shoppingListRepository.GetByIdAsync(id);

    if (shoppingList == null)
    {
        throw new BusinessException("SHOPPING_LIST_NOT_FOUND", "Shopping list not found");
    }

    // Değişiklikleri uygula
    shoppingList.Name = name;
    if (shoppingList.Items.All(i => i.IsChecked))
    {
        isCompleted = true;
    }
    shoppingList.IsCompleted = isCompleted; // <-- ARTIK BU DA GÜNCELLENİYOR

    await _shoppingListRepository.UpdateAsync(shoppingList);
}

    public async Task DeleteShoppingListAsync(int id)
    {
        var shoppingList = await _shoppingListRepository.GetByIdAsync(id);

        if (shoppingList == null)
        {
            throw new BusinessException("SHOPPING_LIST_NOT_FOUND", "Shopping list not found");
        }
        else
        {
            await _shoppingListRepository.DeleteAsync(shoppingList);
            
        }
    }

    public async Task AddItemToShoppingListAsync(int shoppingListId, int productId, decimal quantity)
    {
         var shoppingList = await _shoppingListRepository.GetByIdAsync(shoppingListId);
         var product = await _productRepository.GetByIdAsync(productId);

         if(shoppingList == null)
         {
            throw new BusinessException("SHOPPING_LIST_NOT_FOUND", "Shopping list not found");
         }
         if(product == null)
         {
            throw new BusinessException("PRODUCT_NOT_FOUND", "Product not found");
         }

          shoppingList.Items.Add(new ShoppingListProduct
         {
            ProductId = productId,
            ShoppingListId = shoppingListId,
            Quantity = quantity,
            IsChecked = false
         });
         
         await _shoppingListRepository.SaveChangesAsync();
    }

    public async Task RemoveItemFromShoppingListAsync(int shoppingListId, int productId)
    {
        var shoppingList = await _shoppingListRepository.GetByIdAsync(shoppingListId);
        var product = await _productRepository.GetByIdAsync(productId);

        if (shoppingList == null)
        {
            throw new BusinessException("SHOPPING_LIST_NOT_FOUND", "Shopping list not found");
        }
        if (product == null)
        {
            throw new BusinessException("PRODUCT_NOT_FOUND", "Product not found");
        }

        var item = shoppingList.Items.FirstOrDefault(i => i.ProductId == productId);
        if (item == null)
        {
            throw new BusinessException("ITEM_NOT_FOUND", "Item not found in the shopping list");
        }

        shoppingList.Items.Remove(item);
        await _shoppingListRepository.SaveChangesAsync();
    }

    public async Task UpdateItemQuantityAsync(int shoppingListId, int productId, decimal quantity)
    {
        var shoppingList = await _shoppingListRepository.GetByIdAsync(shoppingListId);
        var product = await _productRepository.GetByIdAsync(productId);

        if (shoppingList == null)
        {
            throw new BusinessException("SHOPPING_LIST_NOT_FOUND", "Shopping list not found");
        }
        if (product == null)
        {
            throw new BusinessException("PRODUCT_NOT_FOUND", "Product not found");
        }

        var item = shoppingList.Items.FirstOrDefault(i => i.ProductId == productId);
        if (item == null)
        {
            throw new BusinessException("ITEM_NOT_FOUND", "Item not found in the shopping list");
        }

        item.Quantity = quantity;
        await _shoppingListRepository.SaveChangesAsync();
    }
    public async Task<List<ShoppingListDto>> GetMyShoppingListsAsync(int userId)
    {
        var shoppingLists = await _shoppingListRepository.GetAllByUserIdAsync(userId);
        if (shoppingLists == null)
        {
            throw new BusinessException("SHOPPING_LIST_NOT_FOUND", "Shopping lists not found for the user");
        }
        return shoppingLists.Select(shoppingList => new ShoppingListDto
        {
            Id = shoppingList.Id,
            Name = shoppingList.Name,
            CreatedAt = shoppingList.CreatedAt,
            IsCompleted = shoppingList.IsCompleted,
            Items = shoppingList.Items.Select(item => new ShoppingListProductDto
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                IsChecked = item.IsChecked
            }).ToList()
        }).ToList();
    } 

    public async Task UpdateItemIsCheckedAsync(int shoppingListId, int productId, bool isChecked)
    {
        var shoppingList = await _shoppingListRepository.GetByIdAsync(shoppingListId);
        
        if (shoppingList == null) throw new BusinessException("SHOPPING_LIST_NOT_FOUND", "Shopping list not found");
        
        var item = shoppingList.Items.FirstOrDefault(i => i.ProductId == productId);
        if (item == null)
        {
            throw new BusinessException("ITEM_NOT_FOUND", "Item not found in the shopping list");
        }

        item.IsChecked = isChecked;
        // 2. ADIM: LİSTENİN TAMAMLANDI DURUMUNU GÜNCELLE

        if (shoppingList.Items.Any() && shoppingList.Items.All(i => i.IsChecked))
        {
            shoppingList.IsCompleted = true;
        }
        else
        {
            shoppingList.IsCompleted = false;
        }

        // 3. ADIM: TEK SEFERDE KAYDET
        await _shoppingListRepository.UpdateAsync(shoppingList);
        await _shoppingListRepository.SaveChangesAsync();
    }

    public async Task AddMemberToShoppingListAsync(int shoppingListId, int userIdToInvite, int requesterId)
    {
        // 1. Liste var mı?
        var shoppingList = await _shoppingListRepository.GetByIdAsync(shoppingListId);
        if (shoppingList == null) 
            throw new BusinessException("SHOPPING_LIST_NOT_FOUND", "Shopping list not found");

        // 2. Kullanıcı zaten üye mi?
        if (shoppingList.Members.Any(m => m.UserId == userIdToInvite))
        {
            throw new BusinessException("MEMBER_ALREADY_EXISTS", "User is already a member of the shopping list");
        }

        // 3. GÜVENLİK KONTROLÜ: Bağlantı (Connection) Var mı?
        // Eğer kişi kendini eklemeye çalışmıyorsa (başkası ekliyorsa) bağlantı kontrolü yap
        if( userIdToInvite == requesterId)
        {
            throw new BusinessException("INVALID_OPERATION", "You cannot add yourself as a member.");
        }
        else
        {
            bool areConnected = await _connectionRepository.AreUsersConnectedAsync(requesterId, userIdToInvite);
            
            if (!areConnected)
            {
                throw new BusinessException("NO_CONNECTION", "You can only add users you are connected with.");
            }
        }

        // 4. Her şey tamamsa ekle
        var newMember = new ShoppingListMember
        {
            ShoppingListId = shoppingListId,
            UserId = userIdToInvite,
            JoinedAt = DateTime.UtcNow
        };

        await _shoppingListRepository.AddMemberToListAsync(newMember); // Repository'e bu metodu eklediğini varsayıyorum
    }
}