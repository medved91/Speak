START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'speak') THEN
CREATE SCHEMA speak;
END IF;
END $EF$;

CREATE TABLE IF NOT EXISTS speak."__EFMigrationsHistory" (
                                                       "MigrationId" character varying(150) NOT NULL,
                                                       "ProductVersion" character varying(32) NOT NULL,
                                                       CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

CREATE TABLE speak."CutieMissions" (
                                       "Id" integer GENERATED BY DEFAULT AS IDENTITY,
                                       "Description" text NOT NULL,
                                       CONSTRAINT "PK_CutieMissions" PRIMARY KEY ("Id")
);

CREATE TABLE speak."CutiePlayers" (
                                      "TelegramUsername" text NOT NULL,
                                      "ChatId" bigint NOT NULL,
                                      "FirstName" text NULL,
                                      "LastName" text NULL,
                                      CONSTRAINT "PK_CutiePlayers" PRIMARY KEY ("ChatId", "TelegramUsername")
);

CREATE TABLE speak."CutieThinkingPhrases" (
    "Phrase" text NOT NULL
);

CREATE TABLE speak."ChosenCuties" (
                                      "ChatId" bigint NOT NULL,
                                      "PlayerUsername" text NOT NULL,
                                      "MissionId" integer NOT NULL,
                                      "WhenChosen" timestamp with time zone NOT NULL,
                                      CONSTRAINT "PK_ChosenCuties" PRIMARY KEY ("ChatId", "PlayerUsername", "WhenChosen"),
                                      CONSTRAINT "FK_ChosenCuties_CutieMissions_MissionId" FOREIGN KEY ("MissionId") REFERENCES speak."CutieMissions" ("Id") ON DELETE CASCADE,
                                      CONSTRAINT "FK_ChosenCuties_CutiePlayers_ChatId_PlayerUsername" FOREIGN KEY ("ChatId", "PlayerUsername") REFERENCES speak."CutiePlayers" ("ChatId", "TelegramUsername") ON DELETE CASCADE
);

CREATE INDEX "IX_ChosenCuties_MissionId" ON speak."ChosenCuties" ("MissionId");

INSERT INTO speak."__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20221208123423_Initial', '7.0.0');

COMMIT;
