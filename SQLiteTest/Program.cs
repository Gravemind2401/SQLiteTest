using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteTest
{
    class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    class PersonConfiguration : EntityTypeConfiguration<Person>
    {
        public PersonConfiguration()
        {
            ToTable("person");
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName("id").HasColumnType("integer").IsRequired();
            Property(x => x.Name).HasColumnName("name").HasColumnType("varchar").IsUnicode(false).IsRequired();
        }
    }

    class Context : DbContext
    {
        public DbSet<Person> People { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Configurations.Add(new PersonConfiguration());
        }
    }

    class Program
    {
        const string createSql = @"
create table if not exists person (
    id integer primary key autoincrement not null,
    name varchar(64) not null
)";

        static void Main(string[] args)
        {
            using (var context = new Context())
            {
                context.Database.ExecuteSqlCommand(createSql);

                var people = context.People.ToList();
                if (!people.Any())
                {
                    var person = context.People.Create();
                    person.Id = 0;
                    person.Name = "Test Person";
                    context.People.Add(person);
                    context.SaveChanges();
                }

                var test = context.People.SqlQuery(context.People.Sql);
                var asList = test.ToList();
            }
        }
    }
}
