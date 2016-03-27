using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace EntityFrameworkGames
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (var ctx = new MyContext())
            {
                var q = from n in ctx.Numbers
                        orderby n.Order
                        select new
                        {
                            value = n.Name,
                            translation = n.Translation.Text
                        };
                foreach (var item in q)
                {
                    Console.WriteLine(item.value + "->" + item.translation);
                }
                Console.ReadKey();
            }
        }

        #region dbContext and config    

        public class MyContext : DbContext
        {
            protected override void OnModelCreating(DbModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Number>()
                    .HasKey(p => p.Id)
                    .Property(p => p.Id)
                    .HasColumnOrder(0)
                    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                modelBuilder.Entity<Number>()
                    .Property(p => p.Order)
                    .HasColumnOrder(1);

                modelBuilder.Entity<Translation>()
                    .HasKey(p => p.Id)
                    .Property(p => p.Id)
                    .HasColumnOrder(0)
                    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            }

            public MyContext()
            {
                Database.SetInitializer(new myConf());
            }

            public DbSet<Number> Numbers { get; set; }
            public DbSet<Word> Words { get; set; }
        }

        class myConf : DropCreateDatabaseIfModelChanges<MyContext>
        {
            protected override void Seed(MyContext context)
            {
                var numbers = new List<Number> {
                    new Number { Name = "1", Order = 3, Translation = new Translation { Text = "one" }},
                    new Number { Name = "2", Order = 2, Translation = new Translation {Text = "two"} },
                    new Number { Name = "3", Order = 1 }};

                var words = new List<Word>
                {
                    new Word {Name="car", Order = 1,Translation = new Translation {Text = "eng:car"} },
                    new Word {Name="bike", Order = 2,Translation = new Translation {Text = "eng:bike"} }
                };
                context.Numbers.AddRange(numbers);
                context.Words.AddRange(words);
                base.Seed(context);

            }
        }
        #endregion

        public abstract class OrderedItem
        {
            public int Order { get; set; }
        }

        public class Number : OrderedItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public Translation Translation { get; set; }
        }

        public class Word : OrderedItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public Translation Translation { get; set; }
        }

        public class Translation
        {
            public int Id { get; set; }
            public string Text { get; set; }
        }


    }
}
