﻿// <auto-generated />
using System;
using Halcyon.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Halcyon.Api.Migrations
{
    [DbContext(typeof(HalcyonDbContext))]
    partial class HalcyonDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.HasPostgresExtension(modelBuilder, "pg_trgm");
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Halcyon.Api.Data.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateOnly>("DateOfBirth")
                        .HasColumnType("date")
                        .HasColumnName("date_of_birth");

                    b.Property<string>("EmailAddress")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("email_address");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("first_name");

                    b.Property<bool>("IsLockedOut")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false)
                        .HasColumnName("is_locked_out");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("last_name");

                    b.Property<string>("Password")
                        .HasColumnType("text")
                        .HasColumnName("password");

                    b.Property<Guid?>("PasswordResetToken")
                        .HasColumnType("uuid")
                        .HasColumnName("password_reset_token");

                    b.Property<string[]>("Roles")
                        .HasColumnType("text[]")
                        .HasColumnName("roles");

                    b.Property<string>("Search")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("text")
                        .HasColumnName("search")
                        .HasComputedColumnSql("\"first_name\" || ' ' || \"last_name\" || ' ' || \"email_address\"", true);

                    b.Property<uint>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid")
                        .HasColumnName("xmin");

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.HasIndex("EmailAddress")
                        .IsUnique()
                        .HasDatabaseName("ix_users_email_address");

                    b.HasIndex("Search")
                        .HasDatabaseName("ix_users_search");

                    NpgsqlIndexBuilderExtensions.HasMethod(b.HasIndex("Search"), "gin");
                    NpgsqlIndexBuilderExtensions.HasOperators(b.HasIndex("Search"), new[] { "gin_trgm_ops" });

                    b.ToTable("users", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
