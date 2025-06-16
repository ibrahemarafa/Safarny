using APIs_Graduation.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace APIs_Graduation.Data.Config
{
    public class CityConfigrations : IEntityTypeConfiguration<City>
    {
        public void Configure(EntityTypeBuilder<City> builder)
        {
            builder.Property(p => p.Name).IsRequired();
            //builder.Property(p=> p.Description).IsRequired();
            //builder.Property(p=> p.PictureUrl).IsRequired();

        }
    }
}
