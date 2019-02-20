namespace ConsoleApp2
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    class Kitchen
    {
        Kitchen(int dishes, int workers)
        {
            for (int i = 0; i < dishes; i++)
            {
                this.EmptyDishes.Add(new Dish());
            }

            for (int i = 0; i < workers; i++)
            {
                this.Chefs.Add(new Chef());
            }


        }

        BlockingCollection<Dish> EmptyDishes { get; } = new BlockingCollection<Dish>();

        BlockingCollection<Dish> DishesWithFood { get; } = new BlockingCollection<Dish>();

        BlockingCollection<Dish> DirtyDishes { get; } = new BlockingCollection<Dish>();

        BlockingCollection<Chef> Chefs { get; } = new BlockingCollection<Chef>();

        BlockingCollection<Waiter> Waiters { get; } = new BlockingCollection<Waiter>();

        BlockingCollection<Cleaner> Cleaners { get; } = new BlockingCollection<Cleaner>();
    }

    class Dish
    {
        internal Dish()
        {
            this.Id = Guid.NewGuid();
        }

        private Guid Id { get; }
    }

    abstract class Worker
    {
        private Guid Id { get; }

        internal Worker()
        {
            this.Id = Guid.NewGuid();
        }
    }

    class Chef:Worker
    {
        Task Cook(Dish dish)
        {
            
        }
    }

    class Waiter
    {
    }

    class Cleaner
    {
    }

    class OverCooking
    {

    }
}