START TRANSACTION;

ALTER TABLE speak."ChosenCuties" ADD "ElectionMessageId" integer NULL;

ALTER TABLE speak."ChosenCuties" ADD "MissionResultMessageId" integer NULL;

INSERT INTO speak."__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20221218163951_MissionMessagesIds', '7.0.0');

COMMIT;
