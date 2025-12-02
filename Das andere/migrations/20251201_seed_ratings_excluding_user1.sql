BEGIN;

-- Setze Ratings für alle UserRoute-Einträge außer UserID=1, falls Rating fehlt/ungültig
UPDATE public."UserRoute" ur
SET "Rating" = (floor(random()*5)::int + 1)
WHERE ur."UserID" <> 1
  AND (ur."Rating" IS NULL OR ur."Rating" NOT BETWEEN 1 AND 5);

COMMIT;