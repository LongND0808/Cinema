using BaseInsightDotNet.Commons.Enums;
using Cinema.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Cinema.Infrastructure.Migrations
{
    public partial class seedingdata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var activeStatusId = Guid.NewGuid();
            var inactiveStatusId = Guid.NewGuid();
            var pendingStatusId = Guid.NewGuid();
            var bannedStatusId = Guid.NewGuid();
            var suspendedStatusId = Guid.NewGuid();
            var deletedStatusId = Guid.NewGuid();

            migrationBuilder.InsertData(
                table: "UserStatuses",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[,]
                {
                    { activeStatusId, "Active", "User is active" },
                    { inactiveStatusId, "Inactive", "User is inactive" },
                    { pendingStatusId, "Pending", "User is pending confirmation" },
                    { bannedStatusId, "Banned", "User is banned" },
                    { suspendedStatusId, "Suspended", "User is suspended" },
                    { deletedStatusId, "Deleted", "User account is deleted" }
                });

            var unrankedRankId = Guid.NewGuid();
            var silverRankId = Guid.NewGuid();
            var goldRankId = Guid.NewGuid();
            var platinumRankId = Guid.NewGuid();
            var diamondRankId = Guid.NewGuid();

            migrationBuilder.InsertData(
                table: "RankCustomers",
                columns: new[] { "Id", "Point", "Description", "Name", "IsActive", "Discount" },
                values: new object[,]
                {
                    { unrankedRankId, 0, "Unranked", "Unranked", true, 0.0 },
                    { silverRankId, 100, "Silver rank", "Silver", true, 0.05 },
                    { goldRankId, 1000, "Gold rank", "Gold", true, 0.10 },
                    { platinumRankId, 10000, "Platinum rank", "Platinum", true, 0.15 },
                    { diamondRankId, 100000, "Diamond rank", "Diamond", true, 0.20 }
                });

            var adminUser = new User
            {
                Id = Guid.NewGuid(),
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@admin.com",
                NormalizedEmail = "ADMIN@ADMIN.COM",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                FullName = "Default Admin",
                RankCustomerId = unrankedRankId,
                UserStatusId = activeStatusId,
                Point = 0,
                LockoutEnabled = true,
                AccessFailedCount = 0,
                AvatarUrl = "",
                Gender = Enumerate.Gender.Unknown,
                DateOfBirth = new DateTime(1990, 1, 1),
                isDeleted = false
            };

            var passwordHasher = new PasswordHasher<User>();
            adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "Admin@1234");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] {
                    "Id", "UserName", "NormalizedUserName", "Email", "NormalizedEmail",
                    "EmailConfirmed", "PasswordHash", "SecurityStamp", "ConcurrencyStamp",
                    "PhoneNumber", "PhoneNumberConfirmed", "TwoFactorEnabled", "LockoutEnd",
                    "LockoutEnabled", "AccessFailedCount", "FullName", "RankCustomerId",
                    "UserStatusId", "Point", "AvatarUrl", "Gender", "DateOfBirth", "isDeleted"
                },
                values: new object[]
                {
                    adminUser.Id,
                    adminUser.UserName,
                    adminUser.NormalizedUserName,
                    adminUser.Email,
                    adminUser.NormalizedEmail,
                    adminUser.EmailConfirmed,
                    adminUser.PasswordHash,
                    adminUser.SecurityStamp,
                    adminUser.ConcurrencyStamp,
                    "0956473821",
                    false,
                    false,
                    null,
                    adminUser.LockoutEnabled,
                    adminUser.AccessFailedCount,
                    adminUser.FullName,
                    adminUser.RankCustomerId,
                    adminUser.UserStatusId,
                    adminUser.Point,
                    adminUser.AvatarUrl,
                    (int) adminUser.Gender,
                    adminUser.DateOfBirth,
                    adminUser.isDeleted
                }
            );

            var adminRoleId = Guid.NewGuid();
            var userRoleId = Guid.NewGuid();
            var employeeRoleId = Guid.NewGuid();

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp", "Code" },
                values: new object[,]
                {
                    { adminRoleId, "Admin", "ADMIN", Guid.NewGuid().ToString(), "Admin" },
                    { userRoleId, "User", "USER", Guid.NewGuid().ToString(), "User" },
                    { employeeRoleId, "Employee", "EMPLOYEE", Guid.NewGuid().ToString(), "Employee" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" },
                values: new object[,]
                {
                    { adminUser.Id, adminRoleId },
                    { adminUser.Id, userRoleId },
                    { adminUser.Id, employeeRoleId }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
