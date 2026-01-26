using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Common;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Entities.CBMS;
using DHAFacilitationAPIs.Domain.Entities.GBMS;
using Microsoft.EntityFrameworkCore;

namespace DHAFacilitationAPIs.Infrastructure.Data;
public class OLMRSApplicationDbContext : DbContext, IOLMRSApplicationDbContext
{
    private readonly IUser _loggedInUser;
    public OLMRSApplicationDbContext(DbContextOptions<OLMRSApplicationDbContext> options, IUser loggedInUser) : base(options) 
    { 
        _loggedInUser = loggedInUser;
    }

    // Newly added DbSets
    public DbSet<Club> Clubs => Set<Club>();
    public DbSet<RoomCategory> RoomCategories => Set<RoomCategory>();
    public DbSet<ResidenceType> ResidenceTypes => base.Set<ResidenceType>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<RoomCharge> RoomCharges => Set<RoomCharge>();
    public DbSet<RoomImage> RoomImages => Set<RoomImage>();
    public DbSet<RoomRating> RoomRatings => Set<RoomRating>();
    public DbSet<RoomAvailability> RoomAvailabilities => Set<RoomAvailability>();
    public DbSet<Services> Services => Set<Services>();
    public DbSet<ServiceMapping> ServiceMappings => Set<ServiceMapping>();
    public DbSet<ExtraServiceCharges> ExtraServiceCharges => Set<ExtraServiceCharges>();
    public DbSet<RoomBooking> RoomBookings => Set<RoomBooking>();
    public DbSet<UserClubMembership> UserClubMemberships => Set<UserClubMembership>();
    public DbSet<BookingGuest> BookingGuests => Set<BookingGuest>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<ReservationRoom> ReservationRooms => Set<ReservationRoom>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<PaymentIntent> PaymentIntents => Set<PaymentIntent>();
    public DbSet<RefundPolicy> RefundPolicies => Set<RefundPolicy>();
    public DbSet<RefundRequest> RefundRequests => Set<RefundRequest>();
    public DbSet<ClubBookingStandardTime> ClubBookingStandardTimes => Set<ClubBookingStandardTime>();
    public DbSet<DiscountSetting> DiscountSettings => Set<DiscountSetting>();
    public DbSet<Grounds> Grounds => Set<Grounds>();
    public DbSet<GroundSetting> GroundSettings => Set<GroundSetting>();
    public DbSet<GroundSlots> GroundSlots => Set<GroundSlots>();
    public DbSet<GroundStandtardTime> GroundStandtardTimes => Set<GroundStandtardTime>();
    public DbSet<GroundBooking> GroundBookings => Set<GroundBooking>();
    public DbSet<GroundBookingSlot> GroundBookingSlots => Set<GroundBookingSlot>();
    public DbSet<GroundImages> GroundImages => Set<GroundImages>();
    public DbSet<GroundPaymentIpnLogs> GroundPaymentIpnLogs => Set<GroundPaymentIpnLogs>();
    public DbSet<ClubCategories> ClubCategories => Set<ClubCategories>();
    public DbSet<ClubServiceProcess> ClubProcess => Set<ClubServiceProcess>();
    public DbSet<ClubImages> ClubImages => Set<ClubImages>();
    public DbSet<ClubPrerequisiteDefinitions> ClubPrerequisiteDefinitions => Set<ClubPrerequisiteDefinitions>();
    public DbSet<ClubPrerequisiteOptions> ClubPrerequisiteOptions => Set<ClubPrerequisiteOptions>();
    public DbSet<ClubFeeDefinition> ClubFeeDefinition => Set<ClubFeeDefinition>();
    public DbSet<ClubFeeCategory> ClubFeeCategory => Set<ClubFeeCategory>();
    public DbSet<ClubFeeOption> ClubFeeOption => Set<ClubFeeOption>();


    public new DbSet<TEntity> Set<TEntity>() where TEntity : class
        => base.Set<TEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OLMRSApplicationDbContext).Assembly);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property<int>("Ser")
                    .ValueGeneratedOnAdd()
                    .HasColumnOrder(1);

                modelBuilder.Entity(entityType.ClrType)
                    .Property<Guid>("Id")
                    .HasColumnOrder(2);
            }
        }

        // Add custom index for Room: (ClubId + No) must be unique
        modelBuilder.Entity<Room>()
            .HasIndex(r => new { r.ClubId, r.No })
            .IsUnique();


        modelBuilder.Entity<Room>()
            .HasOne(r => r.Club)
            .WithMany()
            .HasForeignKey(r => r.ClubId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Room>()
            .HasOne(r => r.RoomCategory)
            .WithMany(rc => rc.Rooms)
            .HasForeignKey(r => r.RoomCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Room>()
            .HasOne(r => r.ResidenceType)
            .WithMany(rc => rc.Rooms)
            .HasForeignKey(r => r.ResidenceTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RoomAvailability>()
            .HasOne(ra => ra.Room)
            .WithMany(r => r.Availabilities)
            .HasForeignKey(ra => ra.RoomId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RoomAvailability>()
            .ToTable("RoomAvailabilities");

        modelBuilder.Entity<RoomAvailability>()
            .Property(x => x.FromDate)
            .HasColumnName("FromDate")
            .HasColumnType("datetime2");   // SQL Server

        modelBuilder.Entity<RoomAvailability>()
            .Property(x => x.ToDate)
            .HasColumnName("ToDate")
            .HasColumnType("datetime2");

        modelBuilder.Entity<RoomAvailability>()
            .ToTable(t => t.HasCheckConstraint(
                "CK_RoomAvailability_FromTo",
                "[FromDate] < [ToDate]"      // use <= if you want zero-length ranges to be invalid (current is strict)
            ));

        modelBuilder.Entity<ServiceMapping>()
            .HasIndex(x => new { x.RoomId, x.ServiceId })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0"); // SQL Server


        // (optional) helpful index for range queries
        modelBuilder.Entity<RoomAvailability>()
            .HasIndex(x => new { x.RoomId, x.FromDate, x.ToDate });

        // ✅ GLOBAL FIX: Restrict all cascading relationships
        foreach (var relationship in modelBuilder.Model
            .GetEntityTypes()
            .SelectMany(e => e.GetForeignKeys()))
        {
            // Only override if not explicitly configured above
            if (relationship.DeleteBehavior == DeleteBehavior.Cascade)
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        // Define the Pakistan Standard Time zone
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
        var pakistanTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);

        foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<BaseAuditableEntity> entry in ChangeTracker.Entries<BaseAuditableEntity>())
        {

            string entityName = entry.Metadata.ClrType.Name;

            if (entityName == "UserRefreshToken")
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.Created = pakistanTime;
                        entry.Entity.CreatedBy = _loggedInUser?.Id;
                        entry.Entity.IsActive = true;
                        entry.Entity.IsDeleted = false;
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModified = DateTime.Now;
                        entry.Entity.LastModifiedBy = _loggedInUser?.Id;
                        break;
                }
            }
            else
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.Created = pakistanTime;
                        entry.Entity.CreatedBy = _loggedInUser?.Id;
                        entry.Entity.IsActive = true;
                        entry.Entity.IsDeleted = false;
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModified = pakistanTime;
                        entry.Entity.LastModifiedBy = _loggedInUser?.Id;
                        break;
                }
            }
            // Handle ApplicationLog entries
            foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<ApplicationLog> transactionEntry in ChangeTracker.Entries<ApplicationLog>().Where(e => e.State == EntityState.Added))
            {
                transactionEntry.Entity.CreatedDateTime = pakistanTime;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}
