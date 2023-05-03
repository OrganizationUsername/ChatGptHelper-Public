﻿// <auto-generated />
using System;
using Helper.ServiceGateways;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Helper.ServiceGateways.Migrations
{
    [DbContext(typeof(GptContext))]
    [Migration("20230502005051_images")]
    partial class images
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.0-preview.3.23174.2");

            modelBuilder.Entity("Helper.ServiceGateways.Answer", b =>
                {
                    b.Property<int>("AnswerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("ResponseId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("AnswerId");

                    b.HasIndex("ResponseId");

                    b.ToTable("Answers");
                });

            modelBuilder.Entity("Helper.ServiceGateways.ApiInformation", b =>
                {
                    b.Property<int>("ApiInformationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ApiKey")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("ApiInformationId");

                    b.ToTable("ApiInformations");
                });

            modelBuilder.Entity("Helper.ServiceGateways.ImageQuery", b =>
                {
                    b.Property<int>("ImageQueryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ImageQueryText")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("ImageQueryId");

                    b.ToTable("ImageQueries");
                });

            modelBuilder.Entity("Helper.ServiceGateways.ImageResult", b =>
                {
                    b.Property<int>("ImageResultId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<byte[]>("ImageBlob")
                        .IsRequired()
                        .HasColumnType("BLOB");

                    b.Property<int>("ImageQueryId")
                        .HasColumnType("INTEGER");

                    b.HasKey("ImageResultId");

                    b.HasIndex("ImageQueryId");

                    b.ToTable("ImageResults");
                });

            modelBuilder.Entity("Helper.ServiceGateways.Query", b =>
                {
                    b.Property<int>("QueryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("ResponseId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("TokenCount")
                        .HasColumnType("INTEGER");

                    b.HasKey("QueryId");

                    b.ToTable("Queries");
                });

            modelBuilder.Entity("Helper.ServiceGateways.Response", b =>
                {
                    b.Property<int>("ResponseId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ModelUsed")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("QueryId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TokenCount")
                        .HasColumnType("INTEGER");

                    b.HasKey("ResponseId");

                    b.ToTable("Responses");
                });

            modelBuilder.Entity("Helper.ServiceGateways.Answer", b =>
                {
                    b.HasOne("Helper.ServiceGateways.Response", "Response")
                        .WithMany("Answers")
                        .HasForeignKey("ResponseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Response");
                });

            modelBuilder.Entity("Helper.ServiceGateways.ImageResult", b =>
                {
                    b.HasOne("Helper.ServiceGateways.ImageQuery", "ImageQuery")
                        .WithMany("ImageResults")
                        .HasForeignKey("ImageQueryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ImageQuery");
                });

            modelBuilder.Entity("Helper.ServiceGateways.Response", b =>
                {
                    b.HasOne("Helper.ServiceGateways.Query", "Query")
                        .WithOne("Response")
                        .HasForeignKey("Helper.ServiceGateways.Response", "ResponseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Query");
                });

            modelBuilder.Entity("Helper.ServiceGateways.ImageQuery", b =>
                {
                    b.Navigation("ImageResults");
                });

            modelBuilder.Entity("Helper.ServiceGateways.Query", b =>
                {
                    b.Navigation("Response")
                        .IsRequired();
                });

            modelBuilder.Entity("Helper.ServiceGateways.Response", b =>
                {
                    b.Navigation("Answers");
                });
#pragma warning restore 612, 618
        }
    }
}
