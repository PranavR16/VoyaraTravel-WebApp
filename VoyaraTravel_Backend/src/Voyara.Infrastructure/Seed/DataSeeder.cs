using Microsoft.Extensions.DependencyInjection;
using Voyara.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Voyara.Core.Entities;

namespace Voyara.Infrastructure.Seed
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<VoyaraDbContext>();

            await context.Database.MigrateAsync();

            await SeedDestinationsAsync(context);
            await SeedPackagesAsync(context);
            await SeedCouponsAsync(context);
            await SeedAdminUserAsync(context);
        }

        // ── Destinations ─────────────────────────────────────
        private static async Task SeedDestinationsAsync(VoyaraDbContext context)
        {
            if (await context.Destinations.AnyAsync()) return;

            var destinations = new List<Destination>
        {
            new()
            {
                Id            = Guid.Parse("11111111-0000-0000-0000-000000000001"),
                Name          = "Bali",
                Country       = "Indonesia 🇮🇩",
                Image         = "https://images.unsplash.com/photo-1537996194471-e657df975ab4?w=800&q=80",
                StartingPrice = 45000,
                Description   = "Island of the Gods with stunning temples and beaches",
                IsActive      = true
            },
            new()
            {
                Id            = Guid.Parse("11111111-0000-0000-0000-000000000002"),
                Name          = "Paris",
                Country       = "France 🇫🇷",
                Image         = "https://images.unsplash.com/photo-1502602898657-3e91760cbb34?w=800&q=80",
                StartingPrice = 85000,
                Description   = "City of Love, Art and Cuisine",
                IsActive      = true
            },
            new()
            {
                Id            = Guid.Parse("11111111-0000-0000-0000-000000000003"),
                Name          = "Tokyo",
                Country       = "Japan 🇯🇵",
                Image         = "https://images.unsplash.com/photo-1540959733332-eab4deabeeaf?w=800&q=80",
                StartingPrice = 75000,
                Description   = "Where ancient tradition meets ultra-modern life",
                IsActive      = true
            },
            new()
            {
                Id            = Guid.Parse("11111111-0000-0000-0000-000000000004"),
                Name          = "Dubai",
                Country       = "UAE 🇦🇪",
                Image         = "https://images.unsplash.com/photo-1512453979798-5ea266f8880c?w=800&q=80",
                StartingPrice = 60000,
                Description   = "A city of superlatives and luxury",
                IsActive      = true
            },
            new()
            {
                Id            = Guid.Parse("11111111-0000-0000-0000-000000000005"),
                Name          = "Maldives",
                Country       = "Maldives 🇲🇻",
                Image         = "https://images.unsplash.com/photo-1514282401047-d79a71a590e8?w=800&q=80",
                StartingPrice = 120000,
                Description   = "Pristine paradise of turquoise lagoons",
                IsActive      = true
            },
            new()
            {
                Id            = Guid.Parse("11111111-0000-0000-0000-000000000006"),
                Name          = "Santorini",
                Country       = "Greece 🇬🇷",
                Image         = "https://images.unsplash.com/photo-1570077188670-e3a8d69ac5ff?w=800&q=80",
                StartingPrice = 95000,
                Description   = "White-washed cliffs and volcanic beaches",
                IsActive      = true
            }
        };

            await context.Destinations.AddRangeAsync(destinations);
            await context.SaveChangesAsync();
            Console.WriteLine("✅ Destinations seeded");
        }

        // ── Packages ─────────────────────────────────────────
        private static async Task SeedPackagesAsync(VoyaraDbContext context)
        {
            if (await context.Packages.AnyAsync()) return;

            var packages = new List<Package>
        {
            new()
            {
                Id            = Guid.Parse("22222222-0000-0000-0000-000000000001"),
                Name          = "Maldives Luxury Escape",
                Description   = "Experience the ultimate luxury in overwater bungalows with crystal clear waters.",
                Price         = 125000,
                OldPrice      = 165000,
                Nights        = 7,
                Category      = "luxury",
                Badge         = "HOT DEAL",
                BadgeClass    = "hot",
                Rating        = 4.9,
                ReviewCount   = 2341,
                Discount      = 24,
                Image         = "https://images.unsplash.com/photo-1514282401047-d79a71a590e8?w=800&q=80",
                Unit          = "per person",
                Tags          = ["Flights", "All Meals", "5-Star"],
                Inclusions    = ["Return flights", "Airport transfers", "All meals", "Water sports"],
                DestinationId = Guid.Parse("11111111-0000-0000-0000-000000000005"),
                IsActive      = true
            },
            new()
            {
                Id            = Guid.Parse("22222222-0000-0000-0000-000000000002"),
                Name          = "Japan Cherry Blossom",
                Description   = "Witness Japan's magical cherry blossoms in full bloom across Tokyo and Kyoto.",
                Price         = 89000,
                OldPrice      = 115000,
                Nights        = 10,
                Category      = "cultural",
                Badge         = "NEW",
                BadgeClass    = "new",
                Rating        = 4.8,
                ReviewCount   = 1892,
                Discount      = 22,
                Image         = "https://images.unsplash.com/photo-1540959733332-eab4deabeeaf?w=800&q=80",
                Unit          = "per person",
                Tags          = ["Flights", "5-Star", "Guide"],
                Inclusions    = ["Return flights", "4 cities", "Cultural tours", "Bullet train pass"],
                DestinationId = Guid.Parse("11111111-0000-0000-0000-000000000003"),
                IsActive      = true
            },
            new()
            {
                Id            = Guid.Parse("22222222-0000-0000-0000-000000000003"),
                Name          = "Europe Grand Tour",
                Description   = "Explore the best of Europe from Paris to Rome in one unforgettable journey.",
                Price         = 145000,
                OldPrice      = 180000,
                Nights        = 14,
                Category      = "cultural",
                Badge         = "POPULAR",
                BadgeClass    = "popular",
                Rating        = 4.7,
                ReviewCount   = 3107,
                Discount      = 19,
                Image         = "https://images.unsplash.com/photo-1502602898657-3e91760cbb34?w=800&q=80",
                Unit          = "per person",
                Tags          = ["Flights", "Bus Tour", "B&B"],
                Inclusions    = ["Return flights", "8 countries", "Guided tours", "Breakfast daily"],
                DestinationId = Guid.Parse("11111111-0000-0000-0000-000000000002"),
                IsActive      = true
            },
            new()
            {
                Id            = Guid.Parse("22222222-0000-0000-0000-000000000004"),
                Name          = "Santorini Honeymoon",
                Description   = "Romantic escape with stunning sunsets, private pools and candlelit dinners.",
                Price         = 110000,
                OldPrice      = 140000,
                Nights        = 6,
                Category      = "honeymoon",
                Badge         = "HONEYMOON",
                BadgeClass    = "honeymoon",
                Rating        = 5.0,
                ReviewCount   = 988,
                Discount      = 21,
                Image         = "https://images.unsplash.com/photo-1570077188670-e3a8d69ac5ff?w=800&q=80",
                Unit          = "per couple",
                Tags          = ["Business", "Romantic", "Candlelight"],
                Inclusions    = ["Business class flights", "Private villa", "Couples spa", "Sunset cruise"],
                DestinationId = Guid.Parse("11111111-0000-0000-0000-000000000006"),
                IsActive      = true
            },
            new()
            {
                Id            = Guid.Parse("22222222-0000-0000-0000-000000000005"),
                Name          = "Bali Family Adventure",
                Description   = "Perfect family getaway with temples, rice terraces and beach fun for all ages.",
                Price         = 65000,
                OldPrice      = 82000,
                Nights        = 8,
                Category      = "family",
                Badge         = "FAMILY",
                BadgeClass    = "family",
                Rating        = 4.6,
                ReviewCount   = 1543,
                Discount      = 20,
                Image         = "https://images.unsplash.com/photo-1537996194471-e657df975ab4?w=800&q=80",
                Unit          = "per person",
                Tags          = ["Flights", "Resort", "Kids Club"],
                Inclusions    = ["Return flights", "Family resort", "Kids activities", "Temple tours"],
                DestinationId = Guid.Parse("11111111-0000-0000-0000-000000000001"),
                IsActive      = true
            },
            new()
            {
                Id            = Guid.Parse("22222222-0000-0000-0000-000000000006"),
                Name          = "Dubai Desert Safari",
                Description   = "Experience the thrill of dune bashing, camel rides and Bedouin camp dining.",
                Price         = 55000,
                OldPrice      = 70000,
                Nights        = 5,
                Category      = "adventure",
                Badge         = "ADVENTURE",
                BadgeClass    = "adventure",
                Rating        = 4.5,
                ReviewCount   = 2210,
                Discount      = 21,
                Image         = "https://images.unsplash.com/photo-1512453979798-5ea266f8880c?w=800&q=80",
                Unit          = "per person",
                Tags          = ["Flights", "4x4 Safari", "Camp Dinner"],
                Inclusions    = ["Return flights", "Desert safari", "Camel ride", "Bedouin dinner"],
                DestinationId = Guid.Parse("11111111-0000-0000-0000-000000000004"),
                IsActive      = true
            },
            new()
            {
                Id            = Guid.Parse("22222222-0000-0000-0000-000000000007"),
                Name          = "Bali Beach Bliss",
                Description   = "Relax on Bali's finest beaches with luxury spa treatments and surf lessons.",
                Price         = 48000,
                OldPrice      = 60000,
                Nights        = 6,
                Category      = "beach",
                Badge         = "BEACH",
                BadgeClass    = "beach",
                Rating        = 4.7,
                ReviewCount   = 1876,
                Discount      = 20,
                Image         = "https://images.unsplash.com/photo-1537996194471-e657df975ab4?w=800&q=80",
                Unit          = "per person",
                Tags          = ["Flights", "Spa", "Surf Lessons"],
                Inclusions    = ["Return flights", "Beachfront villa", "Daily spa", "Surf lessons"],
                DestinationId = Guid.Parse("11111111-0000-0000-0000-000000000001"),
                IsActive      = true
            },
            new()
            {
                Id            = Guid.Parse("22222222-0000-0000-0000-000000000008"),
                Name          = "Paris Luxury Sojourn",
                Description   = "Five-star Parisian experience with Michelin dining and private museum tours.",
                Price         = 185000,
                OldPrice      = 230000,
                Nights        = 7,
                Category      = "luxury",
                Badge         = "LUXURY",
                BadgeClass    = "luxury",
                Rating        = 4.9,
                ReviewCount   = 754,
                Discount      = 19,
                Image         = "https://images.unsplash.com/photo-1502602898657-3e91760cbb34?w=800&q=80",
                Unit          = "per person",
                Tags          = ["First Class", "Michelin", "Private Tour"],
                Inclusions    = ["First class flights", "5-star hotel", "Michelin dinners", "Private Louvre tour"],
                DestinationId = Guid.Parse("11111111-0000-0000-0000-000000000002"),
                IsActive      = true
            }
        };

            await context.Packages.AddRangeAsync(packages);
            await context.SaveChangesAsync();
            Console.WriteLine("✅ Packages seeded");
        }

        // ── Coupons ──────────────────────────────────────────
        private static async Task SeedCouponsAsync(VoyaraDbContext context)
        {
            if (await context.Coupons.AnyAsync()) return;

            var coupons = new List<Coupon>
        {
            new()
            {
                Code        = "VOYARA10",
                DiscountPct = 10,
                MaxUses     = 1000,
                UsedCount   = 0,
                ExpiresAt   = DateTime.UtcNow.AddYears(1),
                IsActive    = true
            },
            new()
            {
                Code        = "SUMMER20",
                DiscountPct = 20,
                MaxUses     = 500,
                UsedCount   = 0,
                ExpiresAt   = DateTime.UtcNow.AddMonths(6),
                IsActive    = true
            },
            new()
            {
                Code        = "FIRST15",
                DiscountPct = 15,
                MaxUses     = 200,
                UsedCount   = 0,
                ExpiresAt   = DateTime.UtcNow.AddYears(1),
                IsActive    = true
            }
        };

            await context.Coupons.AddRangeAsync(coupons);
            await context.SaveChangesAsync();
            Console.WriteLine("✅ Coupons seeded");
        }

        // ── Admin User ───────────────────────────────────────
        private static async Task SeedAdminUserAsync(VoyaraDbContext context)
        {
            if (await context.Users.AnyAsync(u => u.Role == "Admin")) return;

            var admin = new User
            {
                Name = "Voyara Admin",
                Email = "admin@voyara.com",
                Phone = "+91 98765 00000",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@1234"),
                Role = "Admin",
                Nationality = "Indian",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await context.Users.AddAsync(admin);
            await context.SaveChangesAsync();
            Console.WriteLine("✅ Admin user seeded — admin@voyara.com / Admin@1234");
        }
    }
    }
