using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Threading;

namespace EntityFrameworkGames
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var ctx = new MyContext())
            {
                var translations = ctx.Translations.Where(t => t.TableName == "Number");
                var q = from n in ctx.Numbers
                        join translation in translations on n.Id equals translation.NumberId into trGr
                        from t in trGr.DefaultIfEmpty()
                        orderby n.Order
                        select new
                        {
                            name = n.Name,
                            translation = t == null ? n.Name : t.Text,
                            table = t.TableName
                        };


                foreach (var item in q)
                {
                    Console.WriteLine(item.name + " -> " + item.translation);
                }
                Console.Read();
            }


        }

        public class MyContext : DbContext
        {
            protected override void OnModelCreating(DbModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Number>()
                    .HasKey(p => p.Id)
                    .Property(p => p.Id)
                    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

                modelBuilder.Entity<Translation>()
                    .HasKey(p => p.Id)
                    .Property(p => p.Id)
                    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            }

            public MyContext()
            {
                Database.SetInitializer(new myConf());
            }

            public DbSet<Number> Numbers { get; set; }
            public DbSet<Translation> Translations { get; set; }
        }

        class myConf : DropCreateDatabaseIfModelChanges<MyContext>
        {
            public myConf()
            {

            }
            protected override void Seed(MyContext context)
            {
                var numbers = new List<Number>() { new Number() { Name = "1", Order = 3 }, new Number() { Name = "2", Order = 2 }, new Number() { Name = "3", Order = 1 } };
                var translations = new List<Translation>()
                {
                    new Translation() {NumberId = 1, TableName = "Number", Text = "one"},
                    new Translation() {NumberId = 2, TableName = "Number", Text = "two"}
                };
                context.Numbers.AddRange(numbers);
                context.Translations.AddRange(translations);
                base.Seed(context);

            }
        }

        public class Number
        {
            [Column(Order = 1)]
            public int Id { get; set; }
            public string Name { get; set; }

            [Column(Order = 2)]
            public int Order { get; set; }
        }

        public class Translation
        {
            public int Id { get; set; }
            public int NumberId { get; set; }
            public string TableName { get; set; }
            public string Text { get; set; }
        }


    }
}
