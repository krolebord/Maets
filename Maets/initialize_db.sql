BEGIN TRANSACTION;

CREATE TABLE "Apps"
(
    "Id"          UNIQUEIDENTIFIER NOT NULL,
    "Title"       NVARCHAR(255)    NOT NULL,
    "Description" NVARCHAR(max)    NOT NULL DEFAULT N'',
    "ReleaseDate" datetimeoffset   NULL,
    "Price"       DECIMAL(8, 2)    NOT NULL,
    "PublisherId" UNIQUEIDENTIFIER NOT NULL,
    "MainImageId" UNIQUEIDENTIFIER NULL,
    CONSTRAINT "apps_id_primary" PRIMARY KEY (Id)
);
CREATE UNIQUE INDEX "apps_title_unique"
    ON "Apps" ("Title");

CREATE TABLE "Companies"
(
    "Id"          UNIQUEIDENTIFIER NOT NULL,
    "Name"        NVARCHAR(255)    NOT NULL,
    "Description" NVARCHAR(max)    NOT NULL DEFAULT N'',
    "PhotoId"     UNIQUEIDENTIFIER NULL,
    CONSTRAINT "companies_id_primary" PRIMARY KEY ("Id")
);

CREATE TABLE "Labels"
(
    "Id"   UNIQUEIDENTIFIER NOT NULL,
    "Name" NVARCHAR(48)     NOT NULL,
    CONSTRAINT "labels_id_primary" PRIMARY KEY ("Id")
);
CREATE UNIQUE INDEX "labels_name_unique"
    ON "Labels" ("Name");

CREATE TABLE "Reviews"
(
    "Id"           UNIQUEIDENTIFIER NOT NULL,
    "Score"        INT              NOT NULL,
    "Title"        NVARCHAR(255)    NOT NULL,
    "Description"  NVARCHAR(max)    NOT NULL DEFAULT N'',
    "AuthorId"     UNIQUEIDENTIFIER NOT NULL,
    "AppId"        UNIQUEIDENTIFIER NOT NULL,
    "CreationDate" datetimeoffset   NOT NULL DEFAULT getdate(),
    CONSTRAINT "reviews_id_primary" PRIMARY KEY ("Id")
);

CREATE TABLE "MediaFiles"
(
    "Id"  UNIQUEIDENTIFIER NOT NULL,
    "Key" NVARCHAR(255)    NOT NULL,
    CONSTRAINT "mediafiles_id_primary" PRIMARY KEY ("Id")
);
CREATE UNIQUE INDEX "mediafiles_key_unique"
    ON "MediaFiles" ("Key");

CREATE TABLE "Apps_Developers"
(
    "Id"        UNIQUEIDENTIFIER NOT NULL,
    "AppId"     UNIQUEIDENTIFIER NOT NULL,
    "CompanyId" UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT "apps_developers_id_primary" PRIMARY KEY ("Id")
);

CREATE TABLE "Apps_Labels"
(
    "Id"      UNIQUEIDENTIFIER NOT NULL,
    "AppId"   UNIQUEIDENTIFIER NOT NULL,
    "LabelId" UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT "apps_labels_id_primary" PRIMARY KEY ("Id")
);

CREATE TABLE "App_Screenshots"
(
    "Id"     UNIQUEIDENTIFIER NOT NULL,
    "FileId" UNIQUEIDENTIFIER NOT NULL,
    "AppId"  UNIQUEIDENTIFIER NOT NULL,
    "Order"  INT NOT NULL DEFAULT 0,
    CONSTRAINT "app_screenshots_id_primary" PRIMARY KEY ("Id")
);

CREATE TABLE "Apps_UserCollection"
(
    "Id"     UNIQUEIDENTIFIER NOT NULL,
    "UserId" UNIQUEIDENTIFIER NOT NULL,
    "AppId"  UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT "app_usercollection_id_primary" PRIMARY KEY ("Id")
);

create table "Users"
(
    "Id"       UNIQUEIDENTIFIER NOT NULL,
    "UserName" NVARCHAR(255)    NOT NULL,
    "AvatarId" UNIQUEIDENTIFIER NULL,
    CONSTRAINT "users_id_primary" PRIMARY KEY (Id),
    CONSTRAINT "users_avatarid_foreign" FOREIGN KEY ("AvatarId") REFERENCES "MediaFiles" ("Id") ON DELETE SET NULL
);

CREATE UNIQUE INDEX "users_username_unique"
    ON "Users" ("UserName");

CREATE TABLE "CompanyEmployees"
(
    "Id"        UNIQUEIDENTIFIER not null,
    "UserId"    UNIQUEIDENTIFIER not null,
    "CompanyId" UNIQUEIDENTIFIER not null,
    constraint "employees_id_primary" PRIMARY KEY (Id),
    constraint "employees_userid_foreign" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE,
    constraint "employees_companyid_foreign" FOREIGN KEY ("CompanyId") REFERENCES "Companies" ("Id") ON DELETE CASCADE
);

ALTER TABLE "Apps"
    ADD
        CONSTRAINT "apps_publisherid_foreign" FOREIGN KEY ("PublisherId") REFERENCES "Companies" ("Id") ON DELETE CASCADE,
        CONSTRAINT "apps_mainimageid_foreign" FOREIGN KEY ("MainImageId") REFERENCES "MediaFiles" ("Id");

ALTER TABLE "Companies"
    ADD
        CONSTRAINT "companies_photoid_foreign" FOREIGN KEY ("PhotoId") REFERENCES "MediaFiles" ("Id");

ALTER TABLE "Reviews"
    ADD
        CONSTRAINT "reviews_appid_foreign" FOREIGN KEY ("AppId") REFERENCES "Apps" ("Id") ON DELETE CASCADE;

ALTER TABLE "Apps_Developers"
    ADD
        CONSTRAINT "apps_developers_appid_foreign" FOREIGN KEY ("AppId") REFERENCES "Apps" ("Id") ON DELETE CASCADE,
        CONSTRAINT "apps_developers_companyid_foreign" FOREIGN KEY ("CompanyId") REFERENCES "Companies" ("Id");

ALTER TABLE "Apps_Labels"
    ADD
        CONSTRAINT "apps_labels_appid_foreign" FOREIGN KEY ("AppId") REFERENCES "Apps" ("Id") ON DELETE CASCADE,
        CONSTRAINT "apps_labels_labelid_foreign" FOREIGN KEY ("LabelId") REFERENCES "Labels" ("Id") ON DELETE CASCADE;

ALTER TABLE "App_Screenshots"
    ADD
        CONSTRAINT "app_screenshots_appid_foreign" FOREIGN KEY ("AppId") REFERENCES "Apps" ("Id") ON DELETE CASCADE,
        CONSTRAINT "app_screenshots_fileid_foreign" FOREIGN KEY ("FileId") REFERENCES "MediaFiles" ("Id") ON DELETE CASCADE;

ALTER TABLE "Apps_UserCollection"
    ADD
        CONSTRAINT "app_usercollection_appid_foreign" FOREIGN KEY ("AppId") REFERENCES "Apps" ("Id") ON DELETE CASCADE,
        CONSTRAINT "app_usercollection_userid_foreign" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE;

Alter TABLE "Reviews"
    ADD
        CONSTRAINT "reviews_authorid_foreign" FOREIGN KEY ("AuthorId") REFERENCES "Users" ("Id") ON DELETE CASCADE;

COMMIT;
