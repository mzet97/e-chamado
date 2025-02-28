﻿using EChamado.Server.Domain.Domains.Orders.ValueObjects;
using EChamado.Server.Domain.Repositories.Orders;

namespace EChamado.Server.Infrastructure.Persistence.Repositories.Orders;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext db) : base(db)
    {
    }
}
