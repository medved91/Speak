﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Speak.Telegram.Postgres;

#nullable disable

namespace Speak.Telegram.Postgres.Migrations
{
    [DbContext(typeof(TelegramBotDbContext))]
    [Migration("20230113091914_Chats")]
    partial class Chats
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("speak")
                .HasAnnotation("ProductVersion", "7.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Speak.Telegram.CommonContracts.Chat", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<long>("TelegramChatId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("TelegramChatId")
                        .IsUnique();

                    b.ToTable("Chats", "speak");
                });

            modelBuilder.Entity("Speak.Telegram.CutieFeature.Contracts.ChosenCutie", b =>
                {
                    b.Property<int>("ChatsTableId")
                        .HasColumnType("integer");

                    b.Property<string>("PlayerUsername")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("WhenChosen")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int?>("ElectionMessageId")
                        .HasColumnType("integer");

                    b.Property<int>("MissionId")
                        .HasColumnType("integer");

                    b.Property<int?>("MissionResultMessageId")
                        .HasColumnType("integer");

                    b.HasKey("ChatsTableId", "PlayerUsername", "WhenChosen");

                    b.HasIndex("MissionId");

                    b.ToTable("ChosenCuties", "speak");
                });

            modelBuilder.Entity("Speak.Telegram.CutieFeature.Contracts.CutieMission", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("CutieMissions", "speak");
                });

            modelBuilder.Entity("Speak.Telegram.CutieFeature.Contracts.CutiePlayer", b =>
                {
                    b.Property<int>("ChatsTableId")
                        .HasColumnType("integer");

                    b.Property<string>("TelegramUsername")
                        .HasColumnType("text");

                    b.Property<string>("FirstName")
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .HasColumnType("text");

                    b.HasKey("ChatsTableId", "TelegramUsername");

                    b.ToTable("CutiePlayers", "speak");
                });

            modelBuilder.Entity("Speak.Telegram.CutieFeature.Contracts.CutieThinkingPhrase", b =>
                {
                    b.Property<string>("Phrase")
                        .IsRequired()
                        .HasColumnType("text");

                    b.ToTable("CutieThinkingPhrases", "speak");
                });

            modelBuilder.Entity("Speak.Telegram.CutieFeature.Contracts.ChosenCutie", b =>
                {
                    b.HasOne("Speak.Telegram.CutieFeature.Contracts.CutieMission", "Mission")
                        .WithMany()
                        .HasForeignKey("MissionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Speak.Telegram.CutieFeature.Contracts.CutiePlayer", "Player")
                        .WithMany()
                        .HasForeignKey("ChatsTableId", "PlayerUsername")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Mission");

                    b.Navigation("Player");
                });

            modelBuilder.Entity("Speak.Telegram.CutieFeature.Contracts.CutiePlayer", b =>
                {
                    b.HasOne("Speak.Telegram.CommonContracts.Chat", "Chat")
                        .WithMany()
                        .HasForeignKey("ChatsTableId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Chat");
                });
#pragma warning restore 612, 618
        }
    }
}
