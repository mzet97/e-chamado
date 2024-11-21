﻿using EChamado.Core.Domains.Orders.ValueObjects;
using EChamado.Core.Repositories.Orders;

namespace EChamado.Infrastructure.Persistence.Repositories.Orders;

public class OrderTypeRepository : Repository<OrderType>, IOrderTypeRepository
{
    public OrderTypeRepository(ApplicationDbContext db) : base(db)
    {
    }
}