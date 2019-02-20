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
        internal Kitchen(int workers)
        {
            for (int i = 0; i < workers; i++)
            {
                this.Chefs.Add(new Chef(this));
                this.Waiters.Add(new Waiter(this));
                this.Cleaners.Add(new Cleaner(this));
            }
        }

        internal void Order()
        {
            this.EmptyDishes.Add(new Dish());
        }

        internal BlockingCollection<Dish> EmptyDishes { get; } = new BlockingCollection<Dish>();

        internal BlockingCollection<Dish> DishesWithFood { get; } = new BlockingCollection<Dish>();

        internal BlockingCollection<Dish> DirtyDishes { get; } = new BlockingCollection<Dish>();

        BlockingCollection<Chef> Chefs { get; } = new BlockingCollection<Chef>();

        BlockingCollection<Waiter> Waiters { get; } = new BlockingCollection<Waiter>();

        BlockingCollection<Cleaner> Cleaners { get; } = new BlockingCollection<Cleaner>();

        internal void Start()
        {
            foreach (var chef in Chefs)
            {
                chef.Start();
            }

            foreach (var waiter in Waiters)
            {
                waiter.Start();
            }

            foreach (var cleaner in Cleaners)
            {
                cleaner.Start();
            }
        }
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

        protected BlockingCollection<Dish> From { get; }

        protected BlockingCollection<Dish> To { get; }

        internal Worker(BlockingCollection<Dish> from, BlockingCollection<Dish> to)
        {
            this.Id = Guid.NewGuid();
            this.From = from;
            this.To = to;
        }

        void Work(Dish dish)
        {
            if (this.To == null)
            {
                return;
            }

            this.To.Add(dish);
        }

        internal async Task Start()
        {
            while (true)
            {
                if (this.From.TryTake(out var dish))
                {
                    this.Work(dish);
                }

                await Task.Delay(500);
            }
        }
    }

    class Chef : Worker
    {
        internal Chef(Kitchen kitchen)
            : base(kitchen.EmptyDishes, kitchen.DishesWithFood)
        {
        }
    }

    class Waiter : Worker
    {
        internal Waiter(Kitchen kitchen)
            : base(kitchen.DishesWithFood, kitchen.DirtyDishes)
        {
        }
    }

    class Cleaner : Worker
    {
        internal Cleaner(Kitchen kitchen)
            : base(kitchen.DirtyDishes, null)
        {
        }
    }

    class OverCooking
    {
        internal static void Simulate()
        {
            Kitchen kitchen = new Kitchen(3);
            kitchen.Start();
            kitchen.Order();
        }
    }
}