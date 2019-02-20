﻿namespace ConsoleApp2
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;

    using Serilog;
    using Serilog.Context;

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
        private static ILogger Logger = Log.Logger.ForContext("Source", nameof(Dish));

        internal Dish()
        {
            this.Id = Guid.NewGuid();
            Logger.Information("Dish {MessageId} generated.", this.Id);
        }

        internal Guid Id { get; }
    }

    abstract class Worker
    {
        private Random rd = new Random();

        protected Guid Id { get; }

        protected BlockingCollection<Dish> From { get; }

        protected BlockingCollection<Dish> To { get; }

        protected ILogger Logger { get; set; }

        internal Worker(BlockingCollection<Dish> from, BlockingCollection<Dish> to)
        {
            this.Id = Guid.NewGuid();
            this.From = from;
            this.To = to;
        }

        void Work(Dish dish)
        {
            using (LogContext.PushProperty("MessageId", dish.Id))
            {
                Logger.Information("Start working on {MessageId}");

                if (this.To == null)
                {
                    Logger.Warning("Ending dish life cycle of {MessageId}");
                    return;
                }

                this.To.Add(dish);
            }
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
            this.Logger = Log.Logger.ForContext("Source", nameof(Chef)).ForContext("Id", this.Id);
        }
    }

    class Waiter : Worker
    {
        internal Waiter(Kitchen kitchen)
            : base(kitchen.DishesWithFood, kitchen.DirtyDishes)
        {
            this.Logger = Log.Logger.ForContext("Source", nameof(Waiter)).ForContext("Id", this.Id);
        }
    }

    class Cleaner : Worker
    {
        internal Cleaner(Kitchen kitchen)
            : base(kitchen.DirtyDishes, null)
        {
            this.Logger = Log.Logger.ForContext("Source", nameof(Cleaner)).ForContext("Id", this.Id);
        }
    }

    class OverCooking
    {
        static OverCooking()
        {
            var log = new LoggerConfiguration().Enrich.FromLogContext().WriteTo.ColoredConsole(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3} {Source} {Id}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();
            Log.Logger = log;
        }

        internal static void Simulate()
        {
            var log = Log.Logger.ForContext("Source", nameof(OverCooking));
            log.Information("Building Kitchen");
            Kitchen kitchen = new Kitchen(3);
            kitchen.Start();
            kitchen.Order();
            kitchen.Order();
            kitchen.Order();
        }
    }
}