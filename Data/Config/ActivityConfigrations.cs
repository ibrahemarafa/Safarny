using APIs_Graduation.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace APIs_Graduation.Data.Config
{
    public class ActivityConfigrations : IEntityTypeConfiguration<Activity>
    {
        public void Configure(EntityTypeBuilder<Activity> builder)
        {
            builder.Property(p => p.Name).IsRequired();
            builder.Property(p => p.PictureUrl).IsRequired();
            builder.HasOne(p => p.TouristPlaces).WithMany().HasForeignKey(p => p.TouristPlacesId);
        }
    }
}
