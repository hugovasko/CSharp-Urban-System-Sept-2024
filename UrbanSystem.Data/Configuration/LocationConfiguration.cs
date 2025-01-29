using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrbanSystem.Data.Models;
using static UrbanSystem.Common.ValidationConstants.Location;

namespace UrbanSystem.Data.Configuration
{
    public class LocationConfiguration : IEntityTypeConfiguration<Location>
    {
        public void Configure(EntityTypeBuilder<Location> builder)
        {
            builder.HasKey(l => l.Id);

            builder.Property(l => l.CityName)
                .IsRequired()
                .HasMaxLength(CityNameMaxLength);

            builder.Property(l => l.StreetName)
                .IsRequired()
                .HasMaxLength(StreetNameMaxLength);

            builder.HasData(SeedLocations());
        }

        private List<Location> SeedLocations()
        {
            return new List<Location>
            {
                new Location { Id = Guid.NewGuid(), CityName = "Blagoevgrad", StreetName = "Main Street", CityPicture = "https://upload.wikimedia.org/wikipedia/commons/thumb/3/30/%D0%91%D0%BB%D0%B0%D0%B3%D0%BE%D0%B5%D0%B2%D0%B3%D1%80%D0%B0%D0%B4_-_panoramio_%2826%29.jpg/1024px-%D0%91%D0%BB%D0%B0%D0%B3%D0%BE%D0%B5%D0%B2%D0%B3%D1%80%D0%B0%D0%B4_-_panoramio_%2826%29.jpg"},
                new Location {Id = Guid.NewGuid(), CityName = "Burgas", StreetName = "Main Street", CityPicture = "https://dynamic-media-cdn.tripadvisor.com/media/photo-o/08/19/fe/30/getlstd-property-photo.jpg?w=1200&h=-1&s=1"},
                new Location {Id = Guid.NewGuid(), CityName = "Dobrich", StreetName = "Main Street", CityPicture = "https://upload.wikimedia.org/wikipedia/commons/thumb/1/18/Dobrich_Sunrise%2C_Winter_2014.JPG/1280px-Dobrich_Sunrise%2C_Winter_2014.JPG"},
                new Location {Id = Guid.NewGuid(), CityName = "Gabrovo", StreetName = "Main Street", CityPicture = "https://upload.wikimedia.org/wikipedia/commons/thumb/c/c0/TownHall_Gabrovo.jpg/1280px-TownHall_Gabrovo.jpg"},
                new Location {Id = Guid.NewGuid(), CityName = "Haskovo", StreetName = "Main Street", CityPicture = "https://upload.wikimedia.org/wikipedia/commons/thumb/0/01/Haskovo2.jpg/1024px-Haskovo2.jpg"},
                new Location {Id = Guid.NewGuid(), CityName = "Kardzhali", StreetName = "Main Street", CityPicture = "https://upload.wikimedia.org/wikipedia/commons/5/5d/%D0%98%D1%81%D1%82%D0%BE%D1%80%D0%B8%D1%87%D0%B5%D1%81%D0%BA%D0%B8%D1%8F%D1%82_%D0%BC%D1%83%D0%B7%D0%B5%D0%B9_%D0%B2_%D0%9A%D1%8A%D1%80%D0%B4%D0%B6%D0%B0%D0%BB%D0%B8.JPG"},
                new Location {Id = Guid.NewGuid(), CityName = "Kyustendil", StreetName = "Main Street", CityPicture = "https://upload.wikimedia.org/wikipedia/commons/thumb/0/09/Kyustendil_25.jpg/1024px-Kyustendil_25.jpg"},
                new Location {Id = Guid.NewGuid(), CityName = "Lovech", StreetName = "Main Street", CityPicture = "https://upload.wikimedia.org/wikipedia/commons/thumb/3/3b/Bulgaria-Lovech-03.jpg/1024px-Bulgaria-Lovech-03.jpg"},
                new Location {Id = Guid.NewGuid(), CityName = "Montana", StreetName = "Main Street", CityPicture = "https://upload.wikimedia.org/wikipedia/commons/4/47/Montana-downtown.jpg"},
                new Location {Id = Guid.NewGuid(), CityName = "Pazardzhik", StreetName = "Main Street", CityPicture = "https://upload.wikimedia.org/wikipedia/commons/thumb/8/88/Pazardzhik_City_Centre.jpg/1024px-Pazardzhik_City_Centre.jpg"},
                new Location {Id = Guid.NewGuid(), CityName = "Pernik", StreetName = "Main Street", CityPicture = "https://upload.wikimedia.org/wikipedia/commons/thumb/e/ec/Pernik-culture-palace-left.jpg/1920px-Pernik-culture-palace-left.jpg"},
                new Location {Id = Guid.NewGuid(), CityName = "Pleven", StreetName = "Main Street", CityPicture = "https://upload.wikimedia.org/wikipedia/commons/thumb/9/91/%D0%9F%D0%BB%D0%B5%D0%B2%D0%B5%D0%BD_%D0%BC%D0%B0%D1%80%D1%82_2014_-_panoramio_%281%29.jpg/1280px-%D0%9F%D0%BB%D0%B5%D0%B2%D0%B5%D0%BD_%D0%BC%D0%B0%D1%80%D1%82_2014_-_panoramio_%281%29.jpg"},
                new Location {Id = Guid.NewGuid(), CityName = "Plovdiv", StreetName = "Main Street", CityPicture = "https://upload.wikimedia.org/wikipedia/commons/thumb/0/0c/Bulgaria_Bulgaria-0785_-_Roman_Theatre_of_Philippopolis_%287432772486%29.jpg/1280px-Bulgaria_Bulgaria-0785_-_Roman_Theatre_of_Philippopolis_%287432772486%29.jpg"},
                new Location {Id = Guid.NewGuid(), CityName = "Razgrad", StreetName = "Main Street", CityPicture = "https://upload.wikimedia.org/wikipedia/commons/thumb/d/d4/%D0%95%D1%82%D0%BD%D0%BE%D0%B3%D1%80%D0%B0%D1%84%D1%81%D0%BA%D0%B8_%D0%BC%D1%83%D0%B7%D0%B5%D0%B9_%D0%B2_%D0%B3%D1%80%D0%B0%D0%B4_%D0%A0%D0%B0%D0%B7%D0%B3%D1%80%D0%B0%D0%B4.jpg/1280px-%D0%95%D1%82%D0%BD%D0%BE%D0%B3%D1%80%D0%B0%D1%84%D1%81%D0%BA%D0%B8_%D0%BC%D1%83%D0%B7%D0%B5%D0%B9_%D0%B2_%D0%B3%D1%80%D0%B0%D0%B4_%D0%A0%D0%B0%D0%B7%D0%B3%D1%80%D0%B0%D0%B4.jpg"},
                new Location {Id = Guid.NewGuid(), CityName = "Ruse", StreetName = "Main Street", CityPicture = "https://upload.wikimedia.org/wikipedia/commons/thumb/3/39/%D0%9E%D0%BF%D0%B5%D1%80%D0%B0%D1%82%D0%B0_%D0%B2_%D0%A0%D1%83%D1%81%D0%B5.jpg/1280px-%D0%9E%D0%BF%D0%B5%D1%80%D0%B0%D1%82%D0%B0_%D0%B2_%D0%A0%D1%83%D1%81%D0%B5.jpg"},
                new Location {Id = Guid.NewGuid(), CityName = "Shumen", StreetName = "Main Street", CityPicture = "https://upload.wikimedia.org/wikipedia/commons/5/51/Shumen_chitalishte_Dobri_Voynikov.jpg"},
                new Location {Id = Guid.NewGuid(), CityName = "Silistra", StreetName = "Main Street", CityPicture = "https://upload.wikimedia.org/wikipedia/commons/thumb/c/c9/Silistra-art-gallery-Minkov.jpg/1024px-Silistra-art-gallery-Minkov.jpg"},
                new Location {Id = Guid.NewGuid(), CityName = "Sliven", StreetName = "Main Street", CityPicture = "https://upload.wikimedia.org/wikipedia/commons/thumb/9/99/Municipality_of_Sliven_Photo.jpg/1280px-Municipality_of_Sliven_Photo.jpg"},
                new Location {Id = Guid.NewGuid(), CityName = "Smolyan", StreetName = "Main Street", CityPicture = "https://upload.wikimedia.org/wikipedia/commons/thumb/e/e0/%D0%A1%D0%BC%D0%BE%D0%BB%D1%8F%D0%BD_2691396959_f63b323fab_o.jpg/1024px-%D0%A1%D0%BC%D0%BE%D0%BB%D1%8F%D0%BD_2691396959_f63b323fab_o.jpg"},
                new Location {Id = Guid.NewGuid(), CityName = "Sofia", StreetName = "Main Street", CityPicture = "https://upload.wikimedia.org/wikipedia/commons/thumb/c/c0/Catedral_de_Alejandro_Nevski_--_2019_--_Sof%C3%ADa%2C_Bulgaria.jpg/1280px-Catedral_de_Alejandro_Nevski_--_2019_--_Sof%C3%ADa%2C_Bulgaria.jpg"},
                new Location {Id = Guid.NewGuid(), CityName = "Stara Zagora", StreetName = "Main Street", CityPicture = "https://upload.wikimedia.org/wikipedia/commons/3/33/Samarsko_Zname_Panorama.jpg"},
                new Location {Id = Guid.NewGuid(), CityName = "Targovishte", StreetName = "Main Street", CityPicture = "https://upload.wikimedia.org/wikipedia/commons/thumb/5/5c/Targovishte-MainSquare.jpg/1280px-Targovishte-MainSquare.jpg"},
                new Location {Id = Guid.NewGuid(), CityName = "Varna", StreetName = "Main Street", CityPicture = "https://upload.wikimedia.org/wikipedia/en/thumb/7/79/Dramatheatrevarna.jpg/1280px-Dramatheatrevarna.jpg"},
                new Location {Id = Guid.NewGuid(), CityName = "Veliko Tarnovo", StreetName = "Main Street", CityPicture = "https://traventuria.com/wp-content/uploads/2016/10/veliko-tarnovo-1.jpg"},
                new Location {Id = Guid.NewGuid(), CityName = "Vidin", StreetName = "Main Street", CityPicture = "https://upload.wikimedia.org/wikipedia/commons/7/70/Theater_House_in_Vidin_%2827460729905%29.jpg"},
                new Location {Id = Guid.NewGuid(), CityName = "Vratsa", StreetName = "Main Street", CityPicture = "https://upload.wikimedia.org/wikipedia/commons/thumb/a/a4/Vratsa_12.jpg/1024px-Vratsa_12.jpg"},
                new Location {Id = Guid.NewGuid(), CityName = "Yambol", StreetName = "Main Street", CityPicture = "https://upload.wikimedia.org/wikipedia/commons/thumb/9/9d/YAMBOL_new_center.jpg/1920px-YAMBOL_new_center.jpg"}
            };
        }
    }
}
