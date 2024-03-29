﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using Web.Samples.OrderManagement.Domain.Entities;

namespace Web.Samples.OrderManagement.Domain.Context
{
    public partial class OrderContext : DbContext
    {
        public OrderContext()
        {
        }

        public OrderContext(DbContextOptions<OrderContext> options)
            : base(options)
        {
        }

        public virtual DbSet<OrderEntity> Orders => _orders;

        private readonly DbSet<OrderEntity> _orders = new OrderDbSet();
    }

    public class OrderDbSet : DbSet<OrderEntity>
    {
        public OrderDbSet()
        {
            Init();
        }

        private void Init()
        {
            foreach (OrderEntity order in _testData)
            {
                Add(order);
            }
        }

        private readonly List<OrderEntity> _testData = new List<OrderEntity>
        {
            new OrderEntity
            {
                OrderId = 1,
                OrderDate = DateTime.UtcNow.AddMonths(-2)
            },
            new OrderEntity
            {
                OrderId = 2,
                OrderDate = DateTime.UtcNow.AddMonths(-1)
            },
            new OrderEntity
            {
                OrderId = 3,
                OrderDate = DateTime.UtcNow
            }
        };

        public override IEntityType EntityType => throw new NotImplementedException();
    }
}
