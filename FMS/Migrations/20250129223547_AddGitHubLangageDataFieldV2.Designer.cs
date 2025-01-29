﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace FMS.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250129223547_AddGitHubLangageDataFieldV2")]
    partial class AddGitHubLangageDataFieldV2
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("FMS.Models.GitHubLangageDataModel", b =>
                {
                    b.Property<int>("id_github_langage_data")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id_github_langage_data"));

                    b.Property<string>("nom_langage")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("nombre_repertoire")
                        .HasColumnType("int");

                    b.HasKey("id_github_langage_data");

                    b.ToTable("GitHubLanguagesData", (string)null);
                });

            modelBuilder.Entity("FMS.Models.UserModel", b =>
                {
                    b.Property<int>("user_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("user_id"));

                    b.Property<string>("password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("user_id");

                    b.ToTable("Users", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
