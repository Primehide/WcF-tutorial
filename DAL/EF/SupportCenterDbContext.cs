using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using SC.BL.Domain;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace SC.DAL.EF
{
    [DbConfigurationType(typeof(SupportCenterDbConfiguration))]
    internal class SupportCenterDbContext : DbContext
    {
        public SupportCenterDbContext()
            :base("SupportCenterDB_EFCodeFirst")
        {
            Configuration.ProxyCreationEnabled = false;
            Configuration.LazyLoadingEnabled = false;
        }

        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<HardwareTicket> HardwareTickets { get; set; }
        public DbSet<TicketResponse> TicketResponses { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //meervouden van tabelnamen verwijderen
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            //cascading delete voor alle relaties uitschakelen
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

            //primary key voor entity ticket instellen
            modelBuilder.Entity<Ticket>().HasKey(t => t.TicketNumber);
        }
    }
}
