using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using System;

#nullable disable

namespace HastaksharSewaAnalytics.Infrastructure.Migrations;

public partial class NormalizeDatabaseStructure : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_DigitalSignDetails_VaultMasters_ValtMasterId",
            table: "DigitalSignDetails");

        migrationBuilder.DropIndex(
            name: "IX_DigitalSignDetails_ValtMasterId",
            table: "DigitalSignDetails");

        migrationBuilder.RenameColumn(
            name: "ValtMasterId",
            table: "DigitalSignDetails",
            newName: "VaultMasterId");

        migrationBuilder.CreateTable(
            name: "ApplicationMasters",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                AppCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                AppName = table.Column<string>(type: "text", nullable: false),
                IsActive = table.Column<bool>(type: "boolean", nullable: false),
                CreatedBy = table.Column<string>(type: "text", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                ModifiedBy = table.Column<string>(type: "text", nullable: false),
                ModifiedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ApplicationMasters", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "ClientMasters",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                DomainId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                IPAddress = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                DeviceId = table.Column<int>(type: "integer", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                ModifiedBy = table.Column<string>(type: "text", nullable: false),
                ModifiedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ClientMasters", x => x.Id);
                table.ForeignKey(
                    name: "FK_ClientMasters_Devices_DeviceId",
                    column: x => x.DeviceId,
                    principalTable: "Devices",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "ApplicationVersions",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                ApplicationId = table.Column<int>(type: "integer", nullable: false),
                Version = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                CreatedBy = table.Column<string>(type: "text", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                ModifiedBy = table.Column<string>(type: "text", nullable: false),
                ModifiedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
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

        migrationBuilder.CreateIndex(
            name: "IX_ApplicationMasters_AppCode",
            table: "ApplicationMasters",
            column: "AppCode",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_ApplicationVersions_ApplicationId_Version",
            table: "ApplicationVersions",
            columns: new[] { "ApplicationId", "Version" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_ClientMasters_DomainId",
            table: "ClientMasters",
            column: "DomainId",
            unique: true);

        migrationBuilder.AddColumn<int>(
            name: "ClientId",
            table: "HastaksharSewaDailyRunLogs",
            type: "integer",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "VersionId",
            table: "HastaksharSewaDailyRunLogs",
            type: "integer",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "ClientId",
            table: "HastaksharSewaInstallations",
            type: "integer",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "VersionId",
            table: "HastaksharSewaInstallations",
            type: "integer",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "ClientId",
            table: "ClientErrorLogs",
            type: "integer",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "ApplicationVersionId",
            table: "ClientErrorLogs",
            type: "integer",
            nullable: true);

        migrationBuilder.AddColumn<Guid>(
            name: "RequestId",
            table: "ClientErrorLogs",
            type: "uuid",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "DocumentHash",
            table: "DigitalSignDetails",
            type: "character varying(256)",
            maxLength: 256,
            nullable: true);

        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "SignDateTimeUtc",
            table: "DigitalSignDetails",
            type: "timestamp with time zone",
            nullable: true);

        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "ValidFromUtc",
            table: "VaultMasters",
            type: "timestamp with time zone",
            nullable: true);

        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "ValidToUtc",
            table: "VaultMasters",
            type: "timestamp with time zone",
            nullable: true);
        migrationBuilder.Sql(
            "CREATE INDEX IF NOT EXISTS \"IX_DailyRunLogs_DomainId\" ON \"HastaksharSewaDailyRunLogs\"(\"DomainId\");"
        );

        migrationBuilder.Sql(
            "SET statement_timeout = 300000;"
        );
        migrationBuilder.Sql("""
            UPDATE "HastaksharSewaDailyRunLogs"
            SET "DomainId" = 'LEGACY-DAILY-' || "Id"::text
            WHERE "DomainId" IS NULL OR BTRIM("DomainId") = '';

            UPDATE "HastaksharSewaDailyRunLogs"
            SET "Version" = 'UNKNOWN'
            WHERE "Version" IS NULL OR BTRIM("Version") = '';

            UPDATE "HastaksharSewaInstallations"
            SET "DomainId" = 'LEGACY-INSTALL-' || "Id"::text
            WHERE "DomainId" IS NULL OR BTRIM("DomainId") = '';

            UPDATE "HastaksharSewaInstallations"
            SET "Version" = 'UNKNOWN'
            WHERE "Version" IS NULL OR BTRIM("Version") = '';

            UPDATE "ClientErrorLogs"
            SET "AppName" = 'Unknown Application'
            WHERE "AppName" IS NULL OR BTRIM("AppName") = '';
            """);

        migrationBuilder.Sql("""
            INSERT INTO "ApplicationMasters"
                ("AppCode", "AppName", "IsActive", "CreatedBy", "CreatedAt", "ModifiedBy", "ModifiedOn")
            SELECT 'HASTAKSHARSEWA', 'Hastakshar Sewa', TRUE, 'Migration', NOW(), 'Migration', NULL::timestamp with time zone
            WHERE NOT EXISTS (
                SELECT 1 FROM "ApplicationMasters" WHERE "AppCode" = 'HASTAKSHARSEWA'
            );

            INSERT INTO "ApplicationMasters"
                ("AppCode", "AppName", "IsActive", "CreatedBy", "CreatedAt", "ModifiedBy", "ModifiedOn")
            SELECT DISTINCT
                'APP_' || UPPER(SUBSTR(MD5(UPPER(BTRIM("AppName"))), 1, 12)),
                BTRIM("AppName"),
                TRUE,
                'Migration',
                NOW(),
                'Migration',
                NULL::timestamp with time zone
            FROM "ClientErrorLogs"
            WHERE "AppName" IS NOT NULL AND BTRIM("AppName") <> ''
            ON CONFLICT ("AppCode") DO NOTHING;

            INSERT INTO "ApplicationVersions"
                ("ApplicationId", "Version", "CreatedBy", "CreatedAt", "ModifiedBy", "ModifiedOn")
            SELECT a."Id", v."Version", 'Migration', NOW(), 'Migration', NULL::timestamp with time zone
            FROM "ApplicationMasters" a
            CROSS JOIN (
                SELECT DISTINCT BTRIM("Version") AS "Version"
                FROM "HastaksharSewaDailyRunLogs"
                WHERE "Version" IS NOT NULL AND BTRIM("Version") <> ''
                UNION
                SELECT DISTINCT BTRIM("Version") AS "Version"
                FROM "HastaksharSewaInstallations"
                WHERE "Version" IS NOT NULL AND BTRIM("Version") <> ''
            ) v
            WHERE a."AppCode" = 'HASTAKSHARSEWA'
            ON CONFLICT ("ApplicationId", "Version") DO NOTHING;

            INSERT INTO "ApplicationVersions"
                ("ApplicationId", "Version", "CreatedBy", "CreatedAt", "ModifiedBy", "ModifiedOn")
            SELECT DISTINCT
                a."Id",
                COALESCE(NULLIF(BTRIM(e."AppVersion"), ''), 'UNKNOWN'),
                'Migration', NOW(), 'Migration', NULL::timestamp with time zone
            FROM "ClientErrorLogs" e
            JOIN "ApplicationMasters" a
              ON a."AppCode" = 'APP_' || UPPER(SUBSTR(MD5(UPPER(BTRIM(e."AppName"))), 1, 12))
            WHERE e."AppName" IS NOT NULL AND BTRIM(e."AppName") <> ''
            ON CONFLICT ("ApplicationId", "Version") DO NOTHING;
            """);

        migrationBuilder.Sql("""
            WITH source AS (
                SELECT BTRIM("DomainId") AS "DomainId",
                       COALESCE(NULLIF(BTRIM("IPAddress"), ''), '0.0.0.0') AS "IPAddress",
                       "CreatedAt" AS "SeenAt"
                FROM "HastaksharSewaDailyRunLogs"
                WHERE "DomainId" IS NOT NULL AND BTRIM("DomainId") <> ''
                UNION ALL
                SELECT BTRIM("DomainId"),
                       COALESCE(NULLIF(BTRIM("IPAddress"), ''), '0.0.0.0'),
                       "CreatedAt"
                FROM "HastaksharSewaInstallations"
                WHERE "DomainId" IS NOT NULL AND BTRIM("DomainId") <> ''
            ), chosen AS (
                SELECT DISTINCT ON ("DomainId") "DomainId", "IPAddress"
                FROM source
                ORDER BY "DomainId", "SeenAt" DESC
            )
            INSERT INTO "ClientMasters"
                ("DomainId", "IPAddress", "DeviceId", "CreatedBy", "CreatedAt", "ModifiedBy", "ModifiedOn")
            SELECT "DomainId", "IPAddress", NULL, 'Migration', NOW(), 'Migration', NULL::timestamp with time zone
            FROM chosen
            ON CONFLICT ("DomainId") DO NOTHING;

            WITH error_clients AS (
                SELECT DISTINCT
                    CASE
                        WHEN LENGTH(COALESCE(NULLIF(BTRIM("MachineName"), ''), 'UNKNOWN-MACHINE')) <= 64
                            THEN COALESCE(NULLIF(BTRIM("MachineName"), ''), 'UNKNOWN-MACHINE')
                        ELSE 'MACHINE_' || UPPER(SUBSTR(MD5(UPPER(BTRIM("MachineName"))), 1, 12))
                    END AS "DomainId",
                    COALESCE(NULLIF(BTRIM("IpAddress"), ''), '0.0.0.0') AS "IPAddress"
                FROM "ClientErrorLogs"
            )
            INSERT INTO "ClientMasters"
                ("DomainId", "IPAddress", "DeviceId", "CreatedBy", "CreatedAt", "ModifiedBy", "ModifiedOn")
            SELECT "DomainId", "IPAddress", NULL, 'Migration', NOW(), 'Migration', NULL::timestamp with time zone
            FROM error_clients
            ON CONFLICT ("DomainId") DO NOTHING;
            """);

        migrationBuilder.Sql("""
            UPDATE "HastaksharSewaDailyRunLogs" d
            SET "ClientId" = c."Id"
            FROM "ClientMasters" c
            WHERE c."DomainId" = BTRIM(d."DomainId");

            UPDATE "HastaksharSewaDailyRunLogs" d
            SET "VersionId" = v."Id"
            FROM "ApplicationVersions" v
            JOIN "ApplicationMasters" a ON a."Id" = v."ApplicationId"
            WHERE a."AppCode" = 'HASTAKSHARSEWA'
              AND v."Version" = BTRIM(d."Version");

            UPDATE "HastaksharSewaInstallations" i
            SET "ClientId" = c."Id"
            FROM "ClientMasters" c
            WHERE c."DomainId" = BTRIM(i."DomainId");

            UPDATE "HastaksharSewaInstallations" i
            SET "VersionId" = v."Id"
            FROM "ApplicationVersions" v
            JOIN "ApplicationMasters" a ON a."Id" = v."ApplicationId"
            WHERE a."AppCode" = 'HASTAKSHARSEWA'
              AND v."Version" = BTRIM(i."Version");

            UPDATE "ClientErrorLogs" e
            SET "ClientId" = c."Id"
            FROM "ClientMasters" c
            WHERE c."DomainId" = CASE
                WHEN LENGTH(COALESCE(NULLIF(BTRIM(e."MachineName"), ''), 'UNKNOWN-MACHINE')) <= 64
                    THEN COALESCE(NULLIF(BTRIM(e."MachineName"), ''), 'UNKNOWN-MACHINE')
                ELSE 'MACHINE_' || UPPER(SUBSTR(MD5(UPPER(BTRIM(e."MachineName"))), 1, 12))
            END;

            UPDATE "ClientErrorLogs" e
            SET "ApplicationVersionId" = v."Id"
            FROM "ApplicationVersions" v
            JOIN "ApplicationMasters" a ON a."Id" = v."ApplicationId"
            WHERE a."AppCode" = 'APP_' || UPPER(SUBSTR(MD5(UPPER(BTRIM(e."AppName"))), 1, 12))
              AND v."Version" = COALESCE(NULLIF(BTRIM(e."AppVersion"), ''), 'UNKNOWN');
            """);

        migrationBuilder.Sql("""
            UPDATE "ClientErrorLogs"
            SET "ErrorMessage" = 'Legacy error without message'
            WHERE "ErrorMessage" IS NULL OR BTRIM("ErrorMessage") = '';
            """);

        migrationBuilder.AlterColumn<string>(
            name: "ErrorMessage",
            table: "ClientErrorLogs",
            type: "text",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "text",
            oldNullable: true);

        migrationBuilder.Sql("""
            DO $$
            DECLARE r RECORD;
            BEGIN
                FOR r IN SELECT "Id", "ValidFrom", "ValidTo" FROM "VaultMasters" LOOP
                    BEGIN
                        UPDATE "VaultMasters"
                        SET "ValidFromUtc" = r."ValidFrom"::timestamptz
                        WHERE "Id" = r."Id";
                    EXCEPTION WHEN OTHERS THEN
                        NULL;
                    END;

                    BEGIN
                        UPDATE "VaultMasters"
                        SET "ValidToUtc" = r."ValidTo"::timestamptz
                        WHERE "Id" = r."Id";
                    EXCEPTION WHEN OTHERS THEN
                        NULL;
                    END;
                END LOOP;

                FOR r IN SELECT "Id", "SignDateTime" FROM "DigitalSignDetails" LOOP
                    BEGIN
                        UPDATE "DigitalSignDetails"
                        SET "SignDateTimeUtc" = r."SignDateTime"::timestamptz
                        WHERE "Id" = r."Id";
                    EXCEPTION WHEN OTHERS THEN
                        NULL;
                    END;
                END LOOP;
            END $$;

            ALTER TABLE "HastaksharSewaDailyRunLogs"
                ALTER COLUMN "RunOnDate" TYPE date
                USING (("RunOnDate" AT TIME ZONE 'UTC')::date);
            """);

        migrationBuilder.Sql("""
            CREATE TABLE IF NOT EXISTS "NormalizationArchive_DailyRunLogs"
                (LIKE "HastaksharSewaDailyRunLogs" INCLUDING ALL);

            INSERT INTO "NormalizationArchive_DailyRunLogs"
            SELECT d.*
            FROM (
                SELECT "Id",
                       ROW_NUMBER() OVER (PARTITION BY "ClientId", "RunOnDate" ORDER BY "Id") AS rn
                FROM "HastaksharSewaDailyRunLogs"
            ) x
            JOIN "HastaksharSewaDailyRunLogs" d ON d."Id" = x."Id"
            WHERE x.rn > 1;

            DELETE FROM "HastaksharSewaDailyRunLogs" d
            USING "HastaksharSewaDailyRunLogs" keep
            WHERE d."Id" > keep."Id"
              AND d."ClientId" = keep."ClientId"
              AND d."RunOnDate" = keep."RunOnDate";

            CREATE TABLE IF NOT EXISTS "NormalizationArchive_Installations"
                (LIKE "HastaksharSewaInstallations" INCLUDING ALL);

            INSERT INTO "NormalizationArchive_Installations"
            SELECT i.*
            FROM (
                SELECT "Id",
                       ROW_NUMBER() OVER (PARTITION BY "ClientId", "VersionId" ORDER BY "Id") AS rn
                FROM "HastaksharSewaInstallations"
            ) x
            JOIN "HastaksharSewaInstallations" i ON i."Id" = x."Id"
            WHERE x.rn > 1;

            DELETE FROM "HastaksharSewaInstallations" i
            USING "HastaksharSewaInstallations" keep
            WHERE i."Id" > keep."Id"
              AND i."ClientId" = keep."ClientId"
              AND i."VersionId" = keep."VersionId";

            CREATE TABLE IF NOT EXISTS "NormalizationArchive_Devices"
                (LIKE "Devices" INCLUDING ALL);

            INSERT INTO "NormalizationArchive_Devices"
            SELECT d.*
            FROM (
                SELECT "Id", ROW_NUMBER() OVER (PARTITION BY "DeviceId" ORDER BY "Id") AS rn
                FROM "Devices"
            ) x
            JOIN "Devices" d ON d."Id" = x."Id"
            WHERE x.rn > 1;

            DELETE FROM "Devices" d
            USING "Devices" keep
            WHERE d."Id" > keep."Id" AND d."DeviceId" = keep."DeviceId";

            CREATE TABLE IF NOT EXISTS "NormalizationArchive_VaultMasters"
                (LIKE "VaultMasters" INCLUDING ALL);

            INSERT INTO "NormalizationArchive_VaultMasters"
            SELECT v.*
            FROM (
                SELECT "Id", ROW_NUMBER() OVER (PARTITION BY "SerialNo" ORDER BY "Id") AS rn
                FROM "VaultMasters"
            ) x
            JOIN "VaultMasters" v ON v."Id" = x."Id"
            WHERE x.rn > 1;

            UPDATE "DigitalSignDetails" ds
            SET "VaultMasterId" = keeper."KeepId"
            FROM (
                SELECT "SerialNo", MIN("Id") AS "KeepId"
                FROM "VaultMasters"
                GROUP BY "SerialNo"
            ) keeper
            JOIN "VaultMasters" oldv ON oldv."SerialNo" = keeper."SerialNo"
            WHERE ds."VaultMasterId" = oldv."Id"
              AND oldv."Id" <> keeper."KeepId";

            DELETE FROM "VaultMasters" v
            USING "VaultMasters" keep
            WHERE v."Id" > keep."Id" AND v."SerialNo" = keep."SerialNo";
            """);

        migrationBuilder.AlterColumn<int>(
            name: "ClientId",
            table: "HastaksharSewaDailyRunLogs",
            type: "integer",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "integer",
            oldNullable: true);

        migrationBuilder.AlterColumn<int>(
            name: "VersionId",
            table: "HastaksharSewaDailyRunLogs",
            type: "integer",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "integer",
            oldNullable: true);

        migrationBuilder.AlterColumn<int>(
            name: "ClientId",
            table: "HastaksharSewaInstallations",
            type: "integer",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "integer",
            oldNullable: true);

        migrationBuilder.AlterColumn<int>(
            name: "VersionId",
            table: "HastaksharSewaInstallations",
            type: "integer",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "integer",
            oldNullable: true);

        migrationBuilder.AlterColumn<int>(
            name: "ClientId",
            table: "ClientErrorLogs",
            type: "integer",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "integer",
            oldNullable: true);

        migrationBuilder.AlterColumn<int>(
            name: "ApplicationVersionId",
            table: "ClientErrorLogs",
            type: "integer",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "integer",
            oldNullable: true);

        migrationBuilder.DropColumn(name: "DomainId", table: "HastaksharSewaDailyRunLogs");
        migrationBuilder.DropColumn(name: "IPAddress", table: "HastaksharSewaDailyRunLogs");
        migrationBuilder.DropColumn(name: "Version", table: "HastaksharSewaDailyRunLogs");

        migrationBuilder.DropColumn(name: "DomainId", table: "HastaksharSewaInstallations");
        migrationBuilder.DropColumn(name: "IPAddress", table: "HastaksharSewaInstallations");
        migrationBuilder.DropColumn(name: "Version", table: "HastaksharSewaInstallations");

        migrationBuilder.DropColumn(name: "AppName", table: "ClientErrorLogs");
        migrationBuilder.DropColumn(name: "AppVersion", table: "ClientErrorLogs");
        migrationBuilder.DropColumn(name: "IpAddress", table: "ClientErrorLogs");

        migrationBuilder.DropColumn(name: "SignDateTime", table: "DigitalSignDetails");
        migrationBuilder.RenameColumn(name: "SignDateTimeUtc", table: "DigitalSignDetails", newName: "SignDateTime");

        migrationBuilder.DropColumn(name: "ValidFrom", table: "VaultMasters");
        migrationBuilder.DropColumn(name: "ValidTo", table: "VaultMasters");
        migrationBuilder.RenameColumn(name: "ValidFromUtc", table: "VaultMasters", newName: "ValidFrom");
        migrationBuilder.RenameColumn(name: "ValidToUtc", table: "VaultMasters", newName: "ValidTo");

        migrationBuilder.CreateIndex(
            name: "IX_ClientMasters_DeviceId",
            table: "ClientMasters",
            column: "DeviceId");

        migrationBuilder.CreateIndex(
            name: "IX_Devices_DeviceId",
            table: "Devices",
            column: "DeviceId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_VaultMasters_SerialNo",
            table: "VaultMasters",
            column: "SerialNo",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_HastaksharSewaDailyRunLogs_ClientId_RunOnDate",
            table: "HastaksharSewaDailyRunLogs",
            columns: new[] { "ClientId", "RunOnDate" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_HastaksharSewaDailyRunLogs_VersionId",
            table: "HastaksharSewaDailyRunLogs",
            column: "VersionId");

        migrationBuilder.CreateIndex(
            name: "IX_HastaksharSewaInstallations_ClientId_VersionId",
            table: "HastaksharSewaInstallations",
            columns: new[] { "ClientId", "VersionId" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_HastaksharSewaInstallations_VersionId",
            table: "HastaksharSewaInstallations",
            column: "VersionId");

        migrationBuilder.CreateIndex(
            name: "IX_ClientErrorLogs_ClientId",
            table: "ClientErrorLogs",
            column: "ClientId");

        migrationBuilder.CreateIndex(
            name: "IX_ClientErrorLogs_ApplicationVersionId",
            table: "ClientErrorLogs",
            column: "ApplicationVersionId");

        migrationBuilder.CreateIndex(
            name: "IX_ClientErrorLogs_RequestId",
            table: "ClientErrorLogs",
            column: "RequestId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_DigitalSignDetails_VaultMasterId",
            table: "DigitalSignDetails",
            column: "VaultMasterId");

        migrationBuilder.AddForeignKey(
            name: "FK_HastaksharSewaDailyRunLogs_ClientMasters_ClientId",
            table: "HastaksharSewaDailyRunLogs",
            column: "ClientId",
            principalTable: "ClientMasters",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_HastaksharSewaDailyRunLogs_ApplicationVersions_VersionId",
            table: "HastaksharSewaDailyRunLogs",
            column: "VersionId",
            principalTable: "ApplicationVersions",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_HastaksharSewaInstallations_ClientMasters_ClientId",
            table: "HastaksharSewaInstallations",
            column: "ClientId",
            principalTable: "ClientMasters",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_HastaksharSewaInstallations_ApplicationVersions_VersionId",
            table: "HastaksharSewaInstallations",
            column: "VersionId",
            principalTable: "ApplicationVersions",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_ClientErrorLogs_ClientMasters_ClientId",
            table: "ClientErrorLogs",
            column: "ClientId",
            principalTable: "ClientMasters",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_ClientErrorLogs_ApplicationVersions_ApplicationVersionId",
            table: "ClientErrorLogs",
            column: "ApplicationVersionId",
            principalTable: "ApplicationVersions",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_DigitalSignDetails_VaultMasters_VaultMasterId",
            table: "DigitalSignDetails",
            column: "VaultMasterId",
            principalTable: "VaultMasters",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey("FK_HastaksharSewaDailyRunLogs_ClientMasters_ClientId", "HastaksharSewaDailyRunLogs");
        migrationBuilder.DropForeignKey("FK_HastaksharSewaDailyRunLogs_ApplicationVersions_VersionId", "HastaksharSewaDailyRunLogs");
        migrationBuilder.DropForeignKey("FK_HastaksharSewaInstallations_ClientMasters_ClientId", "HastaksharSewaInstallations");
        migrationBuilder.DropForeignKey("FK_HastaksharSewaInstallations_ApplicationVersions_VersionId", "HastaksharSewaInstallations");
        migrationBuilder.DropForeignKey("FK_ClientErrorLogs_ClientMasters_ClientId", "ClientErrorLogs");
        migrationBuilder.DropForeignKey("FK_ClientErrorLogs_ApplicationVersions_ApplicationVersionId", "ClientErrorLogs");
        migrationBuilder.DropForeignKey("FK_DigitalSignDetails_VaultMasters_VaultMasterId", "DigitalSignDetails");

        migrationBuilder.DropIndex("IX_Devices_DeviceId", "Devices");
        migrationBuilder.DropIndex("IX_VaultMasters_SerialNo", "VaultMasters");
        migrationBuilder.DropIndex("IX_HastaksharSewaDailyRunLogs_ClientId_RunOnDate", "HastaksharSewaDailyRunLogs");
        migrationBuilder.DropIndex("IX_HastaksharSewaDailyRunLogs_VersionId", "HastaksharSewaDailyRunLogs");
        migrationBuilder.DropIndex("IX_HastaksharSewaInstallations_ClientId_VersionId", "HastaksharSewaInstallations");
        migrationBuilder.DropIndex("IX_HastaksharSewaInstallations_VersionId", "HastaksharSewaInstallations");
        migrationBuilder.DropIndex("IX_ClientErrorLogs_ClientId", "ClientErrorLogs");
        migrationBuilder.DropIndex("IX_ClientErrorLogs_ApplicationVersionId", "ClientErrorLogs");
        migrationBuilder.DropIndex("IX_ClientErrorLogs_RequestId", "ClientErrorLogs");
        migrationBuilder.DropIndex("IX_DigitalSignDetails_VaultMasterId", "DigitalSignDetails");

        migrationBuilder.AddColumn<string>(name: "DomainId", table: "HastaksharSewaDailyRunLogs", type: "character varying(64)", maxLength: 64, nullable: true);
        migrationBuilder.AddColumn<string>(name: "IPAddress", table: "HastaksharSewaDailyRunLogs", type: "text", nullable: true);
        migrationBuilder.AddColumn<string>(name: "Version", table: "HastaksharSewaDailyRunLogs", type: "character varying(32)", maxLength: 32, nullable: true);

        migrationBuilder.AddColumn<string>(name: "DomainId", table: "HastaksharSewaInstallations", type: "text", nullable: true);
        migrationBuilder.AddColumn<string>(name: "IPAddress", table: "HastaksharSewaInstallations", type: "text", nullable: true);
        migrationBuilder.AddColumn<string>(name: "Version", table: "HastaksharSewaInstallations", type: "text", nullable: true);

        migrationBuilder.AddColumn<string>(name: "AppName", table: "ClientErrorLogs", type: "text", nullable: true);
        migrationBuilder.AddColumn<string>(name: "AppVersion", table: "ClientErrorLogs", type: "text", nullable: true);
        migrationBuilder.AddColumn<string>(name: "IpAddress", table: "ClientErrorLogs", type: "text", nullable: true);

        migrationBuilder.Sql(
            "CREATE INDEX IF NOT EXISTS \"IX_DailyRunLogs_DomainId\" ON \"HastaksharSewaDailyRunLogs\"(\"DomainId\");"
        );

        migrationBuilder.Sql(
            "SET statement_timeout = 300000;"
        );
        migrationBuilder.Sql("""
            UPDATE "HastaksharSewaDailyRunLogs" d
            SET "DomainId" = c."DomainId", "IPAddress" = c."IPAddress", "Version" = v."Version"
            FROM "ClientMasters" c, "ApplicationVersions" v
            WHERE d."ClientId" = c."Id" AND d."VersionId" = v."Id";

            UPDATE "HastaksharSewaInstallations" i
            SET "DomainId" = c."DomainId", "IPAddress" = c."IPAddress", "Version" = v."Version"
            FROM "ClientMasters" c, "ApplicationVersions" v
            WHERE i."ClientId" = c."Id" AND i."VersionId" = v."Id";

            UPDATE "ClientErrorLogs" e
            SET "AppName" = a."AppName", "AppVersion" = v."Version", "IpAddress" = c."IPAddress"
            FROM "ClientMasters" c, "ApplicationVersions" v, "ApplicationMasters" a
            WHERE e."ClientId" = c."Id"
              AND e."ApplicationVersionId" = v."Id"
              AND v."ApplicationId" = a."Id";

            ALTER TABLE "HastaksharSewaDailyRunLogs"
                ALTER COLUMN "RunOnDate" TYPE timestamp with time zone
                USING ("RunOnDate"::timestamp AT TIME ZONE 'UTC');

            ALTER TABLE "VaultMasters"
                ALTER COLUMN "ValidFrom" TYPE text USING "ValidFrom"::text,
                ALTER COLUMN "ValidTo" TYPE text USING "ValidTo"::text;

            ALTER TABLE "DigitalSignDetails"
                ALTER COLUMN "SignDateTime" TYPE text USING "SignDateTime"::text;
            """);

        migrationBuilder.AlterColumn<string>(name: "DomainId", table: "HastaksharSewaDailyRunLogs", type: "character varying(64)", maxLength: 64, nullable: false, oldClrType: typeof(string), oldType: "character varying(64)", oldMaxLength: 64, oldNullable: true);
        migrationBuilder.AlterColumn<string>(name: "IPAddress", table: "HastaksharSewaDailyRunLogs", type: "text", nullable: false, oldClrType: typeof(string), oldType: "text", oldNullable: true);
        migrationBuilder.AlterColumn<string>(name: "Version", table: "HastaksharSewaDailyRunLogs", type: "character varying(32)", maxLength: 32, nullable: false, oldClrType: typeof(string), oldType: "character varying(32)", oldMaxLength: 32, oldNullable: true);

        migrationBuilder.AlterColumn<string>(name: "DomainId", table: "HastaksharSewaInstallations", type: "text", nullable: false, oldClrType: typeof(string), oldType: "text", oldNullable: true);
        migrationBuilder.AlterColumn<string>(name: "IPAddress", table: "HastaksharSewaInstallations", type: "text", nullable: false, oldClrType: typeof(string), oldType: "text", oldNullable: true);
        migrationBuilder.AlterColumn<string>(name: "Version", table: "HastaksharSewaInstallations", type: "text", nullable: false, oldClrType: typeof(string), oldType: "text", oldNullable: true);

        migrationBuilder.AlterColumn<string>(name: "AppName", table: "ClientErrorLogs", type: "text", nullable: false, oldClrType: typeof(string), oldType: "text", oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "ErrorMessage",
            table: "ClientErrorLogs",
            type: "text",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "text");

        migrationBuilder.DropColumn("ClientId", "HastaksharSewaDailyRunLogs");
        migrationBuilder.DropColumn("VersionId", "HastaksharSewaDailyRunLogs");
        migrationBuilder.DropColumn("ClientId", "HastaksharSewaInstallations");
        migrationBuilder.DropColumn("VersionId", "HastaksharSewaInstallations");
        migrationBuilder.DropColumn("ClientId", "ClientErrorLogs");
        migrationBuilder.DropColumn("ApplicationVersionId", "ClientErrorLogs");
        migrationBuilder.DropColumn("RequestId", "ClientErrorLogs");
        migrationBuilder.DropColumn("DocumentHash", "DigitalSignDetails");

        migrationBuilder.RenameColumn("VaultMasterId", "DigitalSignDetails", "ValtMasterId");

        migrationBuilder.CreateIndex(
            name: "IX_DigitalSignDetails_ValtMasterId",
            table: "DigitalSignDetails",
            column: "ValtMasterId");

        migrationBuilder.AddForeignKey(
            name: "FK_DigitalSignDetails_VaultMasters_ValtMasterId",
            table: "DigitalSignDetails",
            column: "ValtMasterId",
            principalTable: "VaultMasters",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.DropTable("ApplicationVersions");
        migrationBuilder.DropTable("ClientMasters");
        migrationBuilder.DropTable("ApplicationMasters");

        migrationBuilder.Sql("""
            DROP TABLE IF EXISTS "NormalizationArchive_DailyRunLogs";
            DROP TABLE IF EXISTS "NormalizationArchive_Installations";
            DROP TABLE IF EXISTS "NormalizationArchive_Devices";
            DROP TABLE IF EXISTS "NormalizationArchive_VaultMasters";
            """);
    }
}
