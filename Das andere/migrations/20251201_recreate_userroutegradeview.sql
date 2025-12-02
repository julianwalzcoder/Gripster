BEGIN;

CREATE OR REPLACE VIEW public.userroutegradeview AS
SELECT
  ur."UserID",
  ur."RouteID",
  ur."Rating",
  ur."Status",
  r."GymID" AS GymID,           -- echte GymID kommt aus Route/Climb-Tabelle
  r."Name"   AS RouteName
FROM public."UserRoute" ur
JOIN public."Route" r ON r."ID" = ur."RouteID";

COMMIT;