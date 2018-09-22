using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RegistrationState;

namespace Registration.Data
{
    public class RegistrationStateInstanceMap : IEntityTypeConfiguration<RegistrationStateInstance>
    {
        public void Configure(EntityTypeBuilder<RegistrationStateInstance> builder)
        {
            builder.Property(x => x.ParticipantEmailAddress).HasMaxLength(256);
            builder.Property(x => x.ParticipantCategory).HasMaxLength(20);
            builder.Property(x => x.ParticipantLicenseNumber).HasMaxLength(20);
            builder.Property(x => x.EventId).HasMaxLength(60);
            builder.Property(x => x.RaceId).HasMaxLength(60);
        }
    }
}