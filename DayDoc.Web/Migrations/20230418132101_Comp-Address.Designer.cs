﻿// <auto-generated />
using System;
using DayDoc.Web.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DayDoc.Web.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20230418132101_Comp-Address")]
    partial class CompAddress
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.7");

            modelBuilder.Entity("DayDoc.Web.Models.Company", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Address")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<int?>("EdoCompanyId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("EdoId")
                        .HasColumnType("TEXT");

                    b.Property<string>("INN")
                        .HasMaxLength(12)
                        .HasColumnType("TEXT");

                    b.Property<string>("KPP")
                        .HasMaxLength(9)
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("OGRN")
                        .HasMaxLength(15)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("OGRN_Date")
                        .HasColumnType("TEXT");

                    b.Property<string>("Signatory_Basis")
                        .HasColumnType("TEXT");

                    b.Property<string>("Signatory_FirstName")
                        .HasColumnType("TEXT");

                    b.Property<string>("Signatory_LastName")
                        .HasColumnType("TEXT");

                    b.Property<string>("Signatory_MiddleName")
                        .HasColumnType("TEXT");

                    b.Property<string>("Signatory_Position")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("EdoCompanyId");

                    b.ToTable("Companies");
                });

            modelBuilder.Entity("DayDoc.Web.Models.Doc", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("ContragentId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<int>("DocType")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("Dogovor_Date")
                        .HasColumnType("TEXT");

                    b.Property<string>("Dogovor_Num")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("Num")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("OwnCompanyId")
                        .HasColumnType("INTEGER");

                    b.Property<double>("Sum")
                        .HasPrecision(19, 4)
                        .HasColumnType("REAL");

                    b.Property<string>("WorkName")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ContragentId");

                    b.HasIndex("OwnCompanyId");

                    b.ToTable("Docs");
                });

            modelBuilder.Entity("DayDoc.Web.Models.Setting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int?>("OwnCompanyId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("WorkName")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("OwnCompanyId");

                    b.ToTable("Settings");
                });

            modelBuilder.Entity("DayDoc.Web.Models.XmlDoc", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DateAndTime")
                        .HasColumnType("TEXT");

                    b.Property<int>("DocId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("DocId");

                    b.ToTable("XmlDocs");
                });

            modelBuilder.Entity("DayDoc.Web.Models.Company", b =>
                {
                    b.HasOne("DayDoc.Web.Models.Company", "EdoCompany")
                        .WithMany()
                        .HasForeignKey("EdoCompanyId");

                    b.Navigation("EdoCompany");
                });

            modelBuilder.Entity("DayDoc.Web.Models.Doc", b =>
                {
                    b.HasOne("DayDoc.Web.Models.Company", "Contragent")
                        .WithMany()
                        .HasForeignKey("ContragentId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("DayDoc.Web.Models.Company", "OwnCompany")
                        .WithMany()
                        .HasForeignKey("OwnCompanyId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Contragent");

                    b.Navigation("OwnCompany");
                });

            modelBuilder.Entity("DayDoc.Web.Models.Setting", b =>
                {
                    b.HasOne("DayDoc.Web.Models.Company", "OwnCompany")
                        .WithMany()
                        .HasForeignKey("OwnCompanyId");

                    b.Navigation("OwnCompany");
                });

            modelBuilder.Entity("DayDoc.Web.Models.XmlDoc", b =>
                {
                    b.HasOne("DayDoc.Web.Models.Doc", "Doc")
                        .WithMany("XmlDocs")
                        .HasForeignKey("DocId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Doc");
                });

            modelBuilder.Entity("DayDoc.Web.Models.Doc", b =>
                {
                    b.Navigation("XmlDocs");
                });
#pragma warning restore 612, 618
        }
    }
}
