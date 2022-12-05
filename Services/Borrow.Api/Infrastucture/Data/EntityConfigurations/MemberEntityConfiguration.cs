using BookOnline.Borrowing.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookOnline.Borrowing.Api.Infrastucture.Data.EntityConfigurations
{
    public class MemberEntityConfiguration : IEntityTypeConfiguration<Member>
    {
        public void Configure(EntityTypeBuilder<Member> builder)
        {
            builder.ToTable("Member");
            builder.HasKey(m => m.Id);
            builder.Property(m => m.Id)
                .UseHiLo("memberseq");

            builder.Ignore(m => m.DomainEvents);

            builder.Property(m => m.MemberName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(m => m.Status)
                .IsRequired();

            builder.Property(m => m.MemberId)
                .IsRequired();

        }
    }
}
