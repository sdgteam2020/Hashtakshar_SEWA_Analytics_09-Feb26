using System;
using HastaksharSewaAnalytics.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HastaksharSewaAnalytics.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationMasters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AppName = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationMasters", x => x.Id);
                });
            //Insert seed data for ApplicationMasters table
            migrationBuilder.InsertData(
                table: "ApplicationMasters",
                columns: new[] { "Id", "AppName", "IsActive", "CreatedBy", "CreatedAt" },
                values: new object[,]
                {
                    { 1, "HastaksharSewa", true, 1, DateTimeOffset.UtcNow }
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });
            //Insert seed data for AspNetRoles table
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                values: new object[,]
                {
                    { 1, "Admin", "ADMIN", Guid.NewGuid().ToString() }
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ActiveSessionId = table.Column<string>(type: "text", nullable: true),
                    ActiveSessionIssuedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UserName = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });
            //Insert seed data for AspNetUsers table
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "UserName", "NormalizedUserName", "Email", "NormalizedEmail", "EmailConfirmed", "PasswordHash", "SecurityStamp", "ConcurrencyStamp", "PhoneNumber", "PhoneNumberConfirmed", "TwoFactorEnabled", "LockoutEnabled", "AccessFailedCount" },
                values: new object[] { 1, "admin", "ADMIN", "admin@example.com", "ADMIN@EXAMPLE.COM", false, new PasswordHasher<ApplicationUser>().HashPassword(null, "Admin@123"), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "+1111111111", false, false, false, 0 });


            migrationBuilder.CreateTable(
                name: "ClientMasters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DomainId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    IPAddress = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientMasters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationVersions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ApplicationId = table.Column<int>(type: "integer", nullable: false),
                    Version = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationVersions_ApplicationMasters_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "ApplicationMasters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });
            //Insert seed data for ApplicationVersions table
            migrationBuilder.InsertData(
                table: "ApplicationVersions",
                columns: new[] { "Id", "ApplicationId", "Version", "CreatedBy", "CreatedAt" },
                values: new object[,]
                {
                    { 1, 1, "2.0.0.0", 1, DateTimeOffset.UtcNow }
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            //Insert seed data for AspNetRoleClaims table
            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "RoleId", "ClaimType", "ClaimValue" },
                values: new object[] { 1, 1, "Permission", "FullAccess" });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            //Insert seed data for AspNetUserClaims table
            migrationBuilder.InsertData(
                table: "AspNetUserClaims",
                columns: new[] { "Id", "UserId", "ClaimType", "ClaimValue" },
                values: new object[] { 1, 1, "Permission", "FullAccess" });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            //Insert seed data for AspNetUserLogins table
            migrationBuilder.InsertData(
                table: "AspNetUserLogins",
                columns: new[] { "LoginProvider", "ProviderKey", "ProviderDisplayName", "UserId" },
                values: new object[] { "DefaultLoginProvider", "1", "Default Login Provider", 1 });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            //Insert seed data for AspNetUserRoles table
            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" },
                values: new object[] { 1, 1 });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            //Insert seed data for AspNetUserTokens table
            migrationBuilder.InsertData(
                table: "AspNetUserTokens",
                columns: new[] { "UserId", "LoginProvider", "Name", "Value" },
                values: new object[] { 1, "DefaultLoginProvider", "DefaultTokenName", "DefaultTokenValue" });

            migrationBuilder.CreateTable(
                name: "ClientKeys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClientKeyHash = table.Column<string>(type: "text", nullable: false),
                    ClientKeySalt = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedByClientId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientKeys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientKeys_ClientMasters_CreatedByClientId",
                        column: x => x.CreatedByClientId,
                        principalTable: "ClientMasters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VaultMasters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Public_Key = table.Column<string>(type: "text", nullable: false),
                    SerialNo = table.Column<string>(type: "text", nullable: false),
                    TokenValid = table.Column<bool>(type: "boolean", nullable: false),
                    ValidFrom = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ValidTo = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedByClientId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VaultMasters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VaultMasters_ClientMasters_CreatedByClientId",
                        column: x => x.CreatedByClientId,
                        principalTable: "ClientMasters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ClientErrorLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ApplicationVersionId = table.Column<int>(type: "integer", nullable: false),
                    ErrorMessage = table.Column<string>(type: "text", nullable: false),
                    CreatedByClientId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientErrorLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientErrorLogs_ApplicationVersions_ApplicationVersionId",
                        column: x => x.ApplicationVersionId,
                        principalTable: "ApplicationVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientErrorLogs_ClientMasters_CreatedByClientId",
                        column: x => x.CreatedByClientId,
                        principalTable: "ClientMasters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HastaksharSewaInstallations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VersionId = table.Column<int>(type: "integer", nullable: false),
                    CreatedByClientId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HastaksharSewaInstallations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HastaksharSewaInstallations_ApplicationVersions_VersionId",
                        column: x => x.VersionId,
                        principalTable: "ApplicationVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HastaksharSewaInstallations_ClientMasters_CreatedByClientId",
                        column: x => x.CreatedByClientId,
                        principalTable: "ClientMasters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DigitalSignDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VaultMasterId = table.Column<int>(type: "integer", nullable: false),
                    SignDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DocumentName = table.Column<string>(type: "text", nullable: true),
                    CreatedByClientId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DigitalSignDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DigitalSignDetails_ClientMasters_CreatedByClientId",
                        column: x => x.CreatedByClientId,
                        principalTable: "ClientMasters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DigitalSignDetails_VaultMasters_VaultMasterId",
                        column: x => x.VaultMasterId,
                        principalTable: "VaultMasters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HastaksharSewaDailyRunLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedByClientId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HastaksharSewaDailyRunLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HastaksharSewaDailyRunLogs_HastaksharSewaInstallations_Crea~",
                        column: x => x.CreatedByClientId,
                        principalTable: "HastaksharSewaInstallations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationVersions_ApplicationId_Version",
                table: "ApplicationVersions",
                columns: new[] { "ApplicationId", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientErrorLogs_ApplicationVersionId",
                table: "ClientErrorLogs",
                column: "ApplicationVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientErrorLogs_CreatedByClientId",
                table: "ClientErrorLogs",
                column: "CreatedByClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientKeys_CreatedByClientId",
                table: "ClientKeys",
                column: "CreatedByClientId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientMasters_DomainId",
                table: "ClientMasters",
                column: "DomainId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DigitalSignDetails_CreatedByClientId",
                table: "DigitalSignDetails",
                column: "CreatedByClientId");

            migrationBuilder.CreateIndex(
                name: "IX_DigitalSignDetails_VaultMasterId",
                table: "DigitalSignDetails",
                column: "VaultMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_HastaksharSewaDailyRunLogs_CreatedByClientId_CreatedAt",
                table: "HastaksharSewaDailyRunLogs",
                columns: new[] { "CreatedByClientId", "CreatedAt" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HastaksharSewaInstallations_CreatedByClientId_VersionId",
                table: "HastaksharSewaInstallations",
                columns: new[] { "CreatedByClientId", "VersionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HastaksharSewaInstallations_VersionId",
                table: "HastaksharSewaInstallations",
                column: "VersionId");

            migrationBuilder.CreateIndex(
                name: "IX_VaultMasters_CreatedByClientId",
                table: "VaultMasters",
                column: "CreatedByClientId");

            migrationBuilder.CreateIndex(
                name: "IX_VaultMasters_SerialNo",
                table: "VaultMasters",
                column: "SerialNo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "ClientErrorLogs");

            migrationBuilder.DropTable(
                name: "ClientKeys");

            migrationBuilder.DropTable(
                name: "DigitalSignDetails");

            migrationBuilder.DropTable(
                name: "HastaksharSewaDailyRunLogs");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "VaultMasters");

            migrationBuilder.DropTable(
                name: "HastaksharSewaInstallations");

            migrationBuilder.DropTable(
                name: "ApplicationVersions");

            migrationBuilder.DropTable(
                name: "ClientMasters");

            migrationBuilder.DropTable(
                name: "ApplicationMasters");
        }
    }
}
