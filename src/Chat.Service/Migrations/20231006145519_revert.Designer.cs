﻿// <auto-generated />
using System;
using Chat.Service.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Chat.Service.Migrations
{
    [DbContext(typeof(ChatDbContext))]
    [Migration("20231006145519_revert")]
    partial class revert
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Chat.Service.Domain.Chats.Aggregates.ChatGroup", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Avatar")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid>("Creator")
                        .HasColumnType("uuid");

                    b.Property<bool>("Default")
                        .HasColumnType("boolean");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(2000)
                        .HasColumnType("character varying(2000)");

                    b.Property<DateTime>("ModificationTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid>("Modifier")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.ToTable("ChatGroups");

                    b.HasData(
                        new
                        {
                            Id = new Guid("eb0549f8-074e-42b0-b727-63316666138b"),
                            Avatar = "https://avatars.githubusercontent.com/u/17716615?v=4",
                            CreationTime = new DateTime(2023, 10, 6, 22, 55, 18, 917, DateTimeKind.Local).AddTicks(3760),
                            Creator = new Guid("00000000-0000-0000-0000-000000000000"),
                            Default = true,
                            Description = "世界频道，所有人默认加入的频道",
                            ModificationTime = new DateTime(2023, 10, 6, 22, 55, 18, 917, DateTimeKind.Local).AddTicks(3761),
                            Modifier = new Guid("00000000-0000-0000-0000-000000000000"),
                            Name = "世界频道"
                        });
                });

            modelBuilder.Entity("Chat.Service.Domain.Chats.Aggregates.ChatGroupInUser", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("ChatGroupId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.HasKey("UserId", "ChatGroupId");

                    b.HasIndex("ChatGroupId");

                    b.HasIndex("Id");

                    b.ToTable("ChatGroupInUsers");

                    b.HasData(
                        new
                        {
                            UserId = new Guid("cf7531f0-ac7d-427d-8393-426cf442b1f2"),
                            ChatGroupId = new Guid("eb0549f8-074e-42b0-b727-63316666138b"),
                            Id = new Guid("00000000-0000-0000-0000-000000000000")
                        });
                });

            modelBuilder.Entity("Chat.Service.Domain.Chats.Aggregates.ChatMessage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("ChatGroupId")
                        .HasColumnType("uuid");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(5000)
                        .HasColumnType("character varying(5000)");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid>("Creator")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("ModificationTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid>("Modifier")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("RevertId")
                        .HasColumnType("uuid");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ChatGroupId");

                    b.HasIndex("Id");

                    b.HasIndex("UserId");

                    b.ToTable("ChatMessages");
                });

            modelBuilder.Entity("Chat.Service.Domain.System.Aggregates.FileSystem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid>("Creator")
                        .HasColumnType("uuid");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("ModificationTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid>("Modifier")
                        .HasColumnType("uuid");

                    b.Property<long>("Size")
                        .HasColumnType("bigint");

                    b.Property<string>("Uri")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.ToTable("FileSystems");
                });

            modelBuilder.Entity("Chat.Service.Domain.Users.Aggregates.Emoji", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid>("Creator")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("ModificationTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid>("Modifier")
                        .HasColumnType("uuid");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Sort")
                        .HasColumnType("integer");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Emojis");
                });

            modelBuilder.Entity("Chat.Service.Domain.Users.Aggregates.Friend", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("FriendId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("GroupId")
                        .HasColumnType("uuid");

                    b.Property<string>("Remark")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("SelfId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.HasIndex("SelfId");

                    b.ToTable("Friends");
                });

            modelBuilder.Entity("Chat.Service.Domain.Users.Aggregates.FriendRequest", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("ApplicationDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid>("BeAppliedForId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid>("Creator")
                        .HasColumnType("uuid");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("ModificationTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid>("Modifier")
                        .HasColumnType("uuid");

                    b.Property<Guid>("RequestId")
                        .HasColumnType("uuid");

                    b.Property<int>("State")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("BeAppliedForId");

                    b.HasIndex("Id");

                    b.HasIndex("RequestId");

                    b.ToTable("FriendRequests");
                });

            modelBuilder.Entity("Chat.Service.Domain.Users.Aggregates.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Account")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.Property<string>("Avatar")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid>("Creator")
                        .HasColumnType("uuid");

                    b.Property<string>("GiteeId")
                        .HasColumnType("text");

                    b.Property<string>("GithubId")
                        .HasColumnType("text");

                    b.Property<DateTime>("ModificationTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid>("Modifier")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(80)
                        .HasColumnType("character varying(80)");

                    b.HasKey("Id");

                    b.HasIndex("Account")
                        .IsUnique();

                    b.HasIndex("Id");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = new Guid("cf7531f0-ac7d-427d-8393-426cf442b1f2"),
                            Account = "admin",
                            Avatar = "https://avatars.githubusercontent.com/u/17716615?v=4",
                            CreationTime = new DateTime(2023, 10, 6, 22, 55, 18, 917, DateTimeKind.Local).AddTicks(3597),
                            Creator = new Guid("00000000-0000-0000-0000-000000000000"),
                            ModificationTime = new DateTime(2023, 10, 6, 22, 55, 18, 917, DateTimeKind.Local).AddTicks(3612),
                            Modifier = new Guid("00000000-0000-0000-0000-000000000000"),
                            Name = "管理员",
                            Password = "3786F993CB0AF43E"
                        });
                });

            modelBuilder.Entity("Chat.Service.Domain.Chats.Aggregates.ChatGroupInUser", b =>
                {
                    b.HasOne("Chat.Service.Domain.Chats.Aggregates.ChatGroup", "ChatGroup")
                        .WithMany()
                        .HasForeignKey("ChatGroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("ChatGroupId");

                    b.HasOne("Chat.Service.Domain.Users.Aggregates.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("UserId");

                    b.Navigation("ChatGroup");

                    b.Navigation("User");
                });
#pragma warning restore 612, 618
        }
    }
}
