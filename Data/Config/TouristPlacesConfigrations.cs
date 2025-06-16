using APIs_Graduation.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace APIs_Graduation.Data.Config
{
    public class TouristPlacesConfigrations : IEntityTypeConfiguration<TouristPlaces>
    {
        public void Configure(EntityTypeBuilder<TouristPlaces> builder)
        {
            builder.Property(p => p.Name).IsRequired();
            builder.Property(p => p.Description).IsRequired();
            builder.Property(p => p.PictureUrl).IsRequired();
            builder.Property(p => p.Category).IsRequired();
            builder.HasOne(p => p.City).WithMany().HasForeignKey(p => p.CityId);
        }
    }
}
