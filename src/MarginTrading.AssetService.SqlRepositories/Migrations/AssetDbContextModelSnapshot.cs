﻿// <auto-generated />
using System;
using MarginTrading.AssetService.SqlRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace MarginTrading.AssetService.SqlRepositories.Migrations
{
    [DbContext(typeof(AssetDbContext))]
    partial class AssetDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("dbo")
                .HasAnnotation("ProductVersion", "6.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("MarginTrading.AssetService.SqlRepositories.Entities.AssetTypeEntity", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("ExcludeSpreadFromProductCosts")
                        .HasColumnType("bit");

                    b.Property<string>("RegulatoryTypeId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UnderlyingCategoryId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("AssetTypes", "dbo");
                });

            modelBuilder.Entity("MarginTrading.AssetService.SqlRepositories.Entities.AuditEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("CorrelationId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DataDiff")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DataReference")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DataType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime2");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("Timestamp");

                    b.ToTable("AuditTrail", "dbo");
                });

            modelBuilder.Entity("MarginTrading.AssetService.SqlRepositories.Entities.ClientProfileEntity", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("IsDefault")
                        .HasColumnType("bit");

                    b.Property<string>("RegulatoryProfileId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ClientProfiles", "dbo");
                });

            modelBuilder.Entity("MarginTrading.AssetService.SqlRepositories.Entities.ClientProfileSettingsEntity", b =>
                {
                    b.Property<string>("ClientProfileId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("AssetTypeId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<decimal>("ExecutionFeesCap")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("ExecutionFeesFloor")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("ExecutionFeesRate")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("FinancingFeesRate")
                        .HasColumnType("decimal(18,2)");

                    b.Property<bool>("IsAvailable")
                        .HasColumnType("bit");

                    b.Property<decimal>("Margin")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("OnBehalfFee")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("ClientProfileId", "AssetTypeId");

                    b.HasIndex("AssetTypeId");

                    b.ToTable("ClientProfileSettings", "dbo");
                });

            modelBuilder.Entity("MarginTrading.AssetService.SqlRepositories.Entities.CurrencyEntity", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("Accuracy")
                        .HasColumnType("int");

                    b.Property<string>("InterestRateMdsCode")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.HasKey("Id");

                    b.ToTable("Currencies", "dbo");
                });

            modelBuilder.Entity("MarginTrading.AssetService.SqlRepositories.Entities.MarketSettingsEntity", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<decimal?>("Dividends871M")
                        .HasColumnType("decimal(18,13)");

                    b.Property<decimal?>("DividendsLong")
                        .HasColumnType("decimal(18,13)");

                    b.Property<decimal?>("DividendsShort")
                        .HasColumnType("decimal(18,13)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NormalizedName")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("MarketSettings", "dbo");
                });

            modelBuilder.Entity("MarginTrading.AssetService.SqlRepositories.Entities.ProductCategoryEntity", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(400)
                        .HasColumnType("nvarchar(400)");

                    b.Property<string>("LocalizationToken")
                        .IsRequired()
                        .HasMaxLength(400)
                        .HasColumnType("nvarchar(400)");

                    b.Property<string>("ParentId")
                        .HasColumnType("nvarchar(400)");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.ToTable("ProductCategories", "dbo");
                });

            modelBuilder.Entity("MarginTrading.AssetService.SqlRepositories.Entities.ProductEntity", b =>
                {
                    b.Property<string>("ProductId")
                        .HasMaxLength(400)
                        .HasColumnType("nvarchar(400)");

                    b.Property<DateOnly?>("ActualDiscontinuedDate")
                        .HasColumnType("date");

                    b.Property<string>("AssetTypeId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CategoryId")
                        .IsRequired()
                        .HasColumnType("nvarchar(400)");

                    b.Property<string>("Comments")
                        .HasMaxLength(400)
                        .HasColumnType("nvarchar(400)");

                    b.Property<int>("ContractSize")
                        .HasMaxLength(400)
                        .HasColumnType("int");

                    b.Property<decimal?>("Dividends871M")
                        .HasColumnType("decimal(18,13)");

                    b.Property<decimal?>("DividendsLong")
                        .HasColumnType("decimal(18,13)");

                    b.Property<decimal?>("DividendsShort")
                        .HasColumnType("decimal(18,13)");

                    b.Property<bool>("EnforceMargin")
                        .HasColumnType("bit");

                    b.Property<string>("ForceId")
                        .IsRequired()
                        .HasMaxLength(400)
                        .HasColumnType("nvarchar(400)");

                    b.Property<string>("FreezeInfo")
                        .HasMaxLength(2000)
                        .HasColumnType("nvarchar(2000)");

                    b.Property<decimal>("HedgeCost")
                        .HasColumnType("decimal(18,2)");

                    b.Property<bool>("IsDiscontinued")
                        .HasColumnType("bit");

                    b.Property<bool>("IsFrozen")
                        .HasColumnType("bit");

                    b.Property<bool>("IsStarted")
                        .HasColumnType("bit");

                    b.Property<bool>("IsSuspended")
                        .HasColumnType("bit");

                    b.Property<bool>("IsTradingDisabled")
                        .HasColumnType("bit");

                    b.Property<string>("IsinLong")
                        .IsRequired()
                        .HasMaxLength(400)
                        .HasColumnType("nvarchar(400)");

                    b.Property<string>("IsinShort")
                        .IsRequired()
                        .HasMaxLength(400)
                        .HasColumnType("nvarchar(400)");

                    b.Property<string>("Issuer")
                        .HasMaxLength(400)
                        .HasColumnType("nvarchar(400)");

                    b.Property<string>("Keywords")
                        .HasMaxLength(400)
                        .HasColumnType("nvarchar(400)");

                    b.Property<decimal?>("Margin")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("MarketId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("MarketMakerAssetAccountId")
                        .HasMaxLength(400)
                        .HasColumnType("nvarchar(400)");

                    b.Property<int>("MaxOrderSize")
                        .HasColumnType("int");

                    b.Property<decimal?>("MaxPositionNotional")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("MaxPositionSize")
                        .HasColumnType("int");

                    b.Property<decimal>("MinOrderDistancePercent")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("MinOrderSize")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(400)
                        .HasColumnType("nvarchar(400)");

                    b.Property<string>("NewsId")
                        .HasMaxLength(400)
                        .HasColumnType("nvarchar(400)");

                    b.Property<decimal>("OvernightMarginMultiplier")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("Parity")
                        .HasColumnType("int");

                    b.Property<string>("PublicationRic")
                        .IsRequired()
                        .HasMaxLength(400)
                        .HasColumnType("nvarchar(400)");

                    b.Property<string>("SettlementCurrency")
                        .HasMaxLength(400)
                        .HasColumnType("nvarchar(400)");

                    b.Property<bool>("ShortPosition")
                        .HasColumnType("bit");

                    b.Property<DateOnly?>("StartDate")
                        .IsRequired()
                        .HasColumnType("date");

                    b.Property<string>("TickFormulaId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.Property<string>("TradingCurrencyId")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("UnderlyingMdsCode")
                        .IsRequired()
                        .HasMaxLength(400)
                        .HasColumnType("nvarchar(400)");

                    b.HasKey("ProductId");

                    b.HasIndex("AssetTypeId");

                    b.HasIndex("CategoryId");

                    b.HasIndex("MarketId");

                    b.HasIndex("TickFormulaId");

                    b.HasIndex("TradingCurrencyId");

                    b.ToTable("Products", "dbo");
                });

            modelBuilder.Entity("MarginTrading.AssetService.SqlRepositories.Entities.TickFormulaEntity", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("PdlLadders")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PdlTicks")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("TickFormulas", "dbo");
                });

            modelBuilder.Entity("MarginTrading.AssetService.SqlRepositories.Entities.ClientProfileSettingsEntity", b =>
                {
                    b.HasOne("MarginTrading.AssetService.SqlRepositories.Entities.AssetTypeEntity", "AssetType")
                        .WithMany()
                        .HasForeignKey("AssetTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MarginTrading.AssetService.SqlRepositories.Entities.ClientProfileEntity", "ClientProfile")
                        .WithMany()
                        .HasForeignKey("ClientProfileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AssetType");

                    b.Navigation("ClientProfile");
                });

            modelBuilder.Entity("MarginTrading.AssetService.SqlRepositories.Entities.MarketSettingsEntity", b =>
                {
                    b.OwnsOne("MarginTrading.AssetService.SqlRepositories.Entities.MarketSettingsEntity.MarketSchedule#MarginTrading.AssetService.SqlRepositories.Entities.MarketScheduleEntity", "MarketSchedule", b1 =>
                        {
                            b1.Property<string>("MarketSettingsEntityId")
                                .HasColumnType("nvarchar(450)");

                            b1.Property<string>("Schedule")
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("MarketSettingsEntityId");

                            b1.ToTable("MarketSettings", "dbo");

                            b1.WithOwner()
                                .HasForeignKey("MarketSettingsEntityId");
                        });

                    b.OwnsMany("MarginTrading.AssetService.SqlRepositories.Entities.MarketSettingsEntity.Holidays#MarginTrading.AssetService.SqlRepositories.Entities.HolidayEntity", "Holidays", b1 =>
                        {
                            b1.Property<DateTime>("Date")
                                .HasColumnType("datetime2");

                            b1.Property<string>("MarketSettingsId")
                                .HasColumnType("nvarchar(450)");

                            b1.HasKey("Date", "MarketSettingsId");

                            b1.HasIndex("MarketSettingsId");

                            b1.ToTable("Holidays", "dbo");

                            b1.WithOwner()
                                .HasForeignKey("MarketSettingsId");
                        });

                    b.Navigation("Holidays");

                    b.Navigation("MarketSchedule")
                        .IsRequired();
                });

            modelBuilder.Entity("MarginTrading.AssetService.SqlRepositories.Entities.ProductCategoryEntity", b =>
                {
                    b.HasOne("MarginTrading.AssetService.SqlRepositories.Entities.ProductCategoryEntity", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("MarginTrading.AssetService.SqlRepositories.Entities.ProductEntity", b =>
                {
                    b.HasOne("MarginTrading.AssetService.SqlRepositories.Entities.AssetTypeEntity", "AssetType")
                        .WithMany()
                        .HasForeignKey("AssetTypeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("MarginTrading.AssetService.SqlRepositories.Entities.ProductCategoryEntity", "Category")
                        .WithMany("Products")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("MarginTrading.AssetService.SqlRepositories.Entities.MarketSettingsEntity", "Market")
                        .WithMany()
                        .HasForeignKey("MarketId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("MarginTrading.AssetService.SqlRepositories.Entities.TickFormulaEntity", "TickFormula")
                        .WithMany()
                        .HasForeignKey("TickFormulaId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("MarginTrading.AssetService.SqlRepositories.Entities.CurrencyEntity", "TradingCurrency")
                        .WithMany()
                        .HasForeignKey("TradingCurrencyId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("AssetType");

                    b.Navigation("Category");

                    b.Navigation("Market");

                    b.Navigation("TickFormula");

                    b.Navigation("TradingCurrency");
                });

            modelBuilder.Entity("MarginTrading.AssetService.SqlRepositories.Entities.ProductCategoryEntity", b =>
                {
                    b.Navigation("Children");

                    b.Navigation("Products");
                });
#pragma warning restore 612, 618
        }
    }
}
