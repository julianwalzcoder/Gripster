-- 1) Ensure trigger does not insert NULL Status
CREATE OR REPLACE FUNCTION public.log_userroute_change()
RETURNS TRIGGER AS $$
BEGIN
  IF NEW."Status" IS NOT NULL THEN
    INSERT INTO public."UserSessionRoute"("UserID","RouteID","Status","LoggedAt")
    VALUES (NEW."UserID", NEW."RouteID", NEW."Status", now());
  END IF;
  RETURN NEW;
END;
$$ LANGUAGE plpgsql;

DROP TRIGGER IF EXISTS trg_log_userroute ON public."UserRoute";
CREATE TRIGGER trg_log_userroute
AFTER INSERT OR UPDATE ON public."UserRoute"
FOR EACH ROW
EXECUTE FUNCTION public.log_userroute_change();

-- 2) Seed Admin mapped to your JWT user (nameidentifier/sub = 8) at an existing Gym (e.g., 1)
INSERT INTO public."Admin"("GymID","UserID")
SELECT 1, 8
WHERE NOT EXISTS (
  SELECT 1 FROM public."Admin" a WHERE a."UserID" = 8
);