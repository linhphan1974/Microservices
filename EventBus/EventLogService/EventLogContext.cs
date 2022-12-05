using BookOnline.EventLogService.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookOnline.EventLogService
{
    public class EventLogContext : DbContext
    {
        public DbSet<ApplicationEventLog> EventLogs { get; set; }

        public EventLogContext(DbContextOptions<EventLogContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ApplicationEventLog>(ConfigureApplicationEventLogEntry);
        }
        void ConfigureApplicationEventLogEntry(EntityTypeBuilder<ApplicationEventLog> builder)
        {
            builder.ToTable("IntegrationEventLog");

            builder.HasKey(e => e.EventId);

            builder.Property(e => e.EventId)
                .IsRequired();

            builder.Property(e => e.Content)
                .IsRequired();

            builder.Property(e => e.CreationTime)
                .IsRequired();

            builder.Property(e => e.State)
                .IsRequired();

            builder.Property(e => e.TimesSent)
                .IsRequired();

            builder.Property(e => e.EventTypeName)
                .IsRequired();

        }


    }
}