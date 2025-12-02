BEGIN;

-- Drop abhängige View(s), falls vorhanden
DO $$
BEGIN
  IF EXISTS (
    SELECT 1 FROM pg_views WHERE schemaname='public' AND viewname='userroutegradeview'
  ) THEN
    EXECUTE 'DROP VIEW public.userroutegradeview';
  END IF;
END$$;

-- 1) Rating-Spalte in UserRoute anlegen
ALTER TABLE public."UserRoute" ADD COLUMN IF NOT EXISTS "Rating" integer;

-- Optional: Wertebereich absichern
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint
        WHERE conname = 'userroute_rating_range'
    ) THEN
        ALTER TABLE public."UserRoute"
        ADD CONSTRAINT userroute_rating_range CHECK ("Rating" BETWEEN 1 AND 5);
    END IF;
END$$;

-- 2) Ratings von SessionRoute auf UserRoute migrieren
-- Annahmen:
-- - SessionRoute("SessionID","RouteID","Rating")
-- - Session("ID","UserID","Date")
WITH latest AS (
    SELECT
        s."UserID",
        sr."RouteID",
        sr."Rating",
        ROW_NUMBER() OVER (PARTITION BY s."UserID", sr."RouteID" ORDER BY s."Date" DESC, sr."RouteID") AS rn
    FROM public."SessionRoute" sr
    JOIN public."Session" s ON s."ID" = sr."SessionID"
    WHERE sr."Rating" IS NOT NULL
)
UPDATE public."UserRoute" ur
SET "Rating" = l."Rating"
FROM latest l
WHERE ur."UserID" = l."UserID"
  AND ur."RouteID" = l."RouteID"
  AND l.rn = 1;

-- Für fehlende UserRoute-Zeilen anlegen (damit alle migrierten Ratings ankommen)
INSERT INTO public."UserRoute" ("UserID","RouteID","Rating")
SELECT l."UserID", l."RouteID", l."Rating"
FROM latest l
LEFT JOIN public."UserRoute" ur
  ON ur."UserID" = l."UserID" AND ur."RouteID" = l."RouteID"
WHERE l.rn = 1 AND ur."UserID" IS NULL;

-- 3) Rating aus SessionRoute entfernen (nach erfolgreicher Migration)
ALTER TABLE public."SessionRoute" DROP COLUMN IF EXISTS "Rating";

-- 4. View anpassen (Rating jetzt aus UserRoute)
CREATE OR REPLACE VIEW public.userroutegradeview AS
SELECT u."ID"    AS userid,
       r."ID"    AS routeid,
       g."FBleau" AS gradefbleau,
       ur."Status" AS status,
       ur."Rating" AS rating,
       r."GymID" AS gymid,
       r."SetDate" AS setdate,
       r."RemoveDate" AS removedate,
       r."AdminID" AS adminid
FROM public."Route" r
JOIN public."Grade" g     ON r."GradeID" = g."ID"
LEFT JOIN public."UserRoute" ur ON r."ID" = ur."RouteID"
LEFT JOIN public."User" u  ON u."ID" = ur."UserID";

COMMIT;