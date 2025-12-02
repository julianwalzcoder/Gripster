--
-- PostgreSQL database dump
--

\restrict mtSjt8fDz3KPVpyFGb2d5hurM7ySlapWYnxweAtnwB5iOjLykssQaaeSAtTM1iu

-- Dumped from database version 17.6 (Postgres.app)
-- Dumped by pg_dump version 17.6 (Postgres.app)

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: pgcrypto; Type: EXTENSION; Schema: -; Owner: -
--

CREATE EXTENSION IF NOT EXISTS pgcrypto WITH SCHEMA public;


--
-- Name: EXTENSION pgcrypto; Type: COMMENT; Schema: -; Owner: 
--

COMMENT ON EXTENSION pgcrypto IS 'cryptographic functions';


SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: Admin; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Admin" (
    "ID" integer NOT NULL,
    "GymID" integer NOT NULL,
    "UserID" integer NOT NULL
);


ALTER TABLE public."Admin" OWNER TO postgres;

--
-- Name: Admin_ID_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."Admin_ID_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."Admin_ID_seq" OWNER TO postgres;

--
-- Name: Admin_ID_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."Admin_ID_seq" OWNED BY public."Admin"."ID";


--
-- Name: Grade; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Grade" (
    "ID" integer NOT NULL,
    "FBleau" character varying(20),
    "VScale" character varying(20)
);


ALTER TABLE public."Grade" OWNER TO postgres;

--
-- Name: Grade_ID_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."Grade_ID_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."Grade_ID_seq" OWNER TO postgres;

--
-- Name: Grade_ID_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."Grade_ID_seq" OWNED BY public."Grade"."ID";


--
-- Name: Gym; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Gym" (
    "ID" integer NOT NULL,
    "Name" character varying(50) NOT NULL,
    "Street" character varying(50),
    "StreetNumber" integer,
    "Postcode" integer,
    "City" character varying(50)
);


ALTER TABLE public."Gym" OWNER TO postgres;

--
-- Name: Gym_ID_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."Gym_ID_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."Gym_ID_seq" OWNER TO postgres;

--
-- Name: Gym_ID_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."Gym_ID_seq" OWNED BY public."Gym"."ID";


--
-- Name: Route; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Route" (
    "ID" integer NOT NULL,
    "GymID" integer NOT NULL,
    "GradeID" integer NOT NULL,
    "SetDate" date,
    "RemoveDate" date,
    "AdminID" integer
);


ALTER TABLE public."Route" OWNER TO postgres;

--
-- Name: Route_ID_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."Route_ID_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."Route_ID_seq" OWNER TO postgres;

--
-- Name: Route_ID_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."Route_ID_seq" OWNED BY public."Route"."ID";


--
-- Name: Session; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Session" (
    "ID" integer NOT NULL,
    "UserID" integer NOT NULL,
    "CustomName" character varying(50),
    "Date" date,
    "Feedback" text
);


ALTER TABLE public."Session" OWNER TO postgres;

--
-- Name: SessionRoute; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."SessionRoute" (
    "SessionID" integer NOT NULL,
    "RouteID" integer NOT NULL,
    "Tries" integer,
    "Rating" integer
);


ALTER TABLE public."SessionRoute" OWNER TO postgres;

--
-- Name: Session_ID_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."Session_ID_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."Session_ID_seq" OWNER TO postgres;

--
-- Name: Session_ID_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."Session_ID_seq" OWNED BY public."Session"."ID";


--
-- Name: User; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."User" (
    "ID" integer NOT NULL,
    "Name" character varying(50),
    "Username" character varying(30) NOT NULL,
    "Mail" character varying(100) NOT NULL,
    "PasswordHash" character varying(100) NOT NULL,
    "Street" character varying(50),
    "StreetNumber" integer,
    "Postcode" integer,
    "City" character varying(50),
    "Role" character varying(50) DEFAULT 'user'::character varying
);


ALTER TABLE public."User" OWNER TO postgres;

--
-- Name: UserRoute; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."UserRoute" (
    "UserID" integer NOT NULL,
    "RouteID" integer NOT NULL,
    "Status" character varying(10)
);


ALTER TABLE public."UserRoute" OWNER TO postgres;

--
-- Name: User_ID_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."User_ID_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."User_ID_seq" OWNER TO postgres;

--
-- Name: User_ID_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."User_ID_seq" OWNED BY public."User"."ID";


--
-- Name: userroutegradeview; Type: VIEW; Schema: public; Owner: postgres
--

CREATE VIEW public.userroutegradeview AS
 SELECT u."ID" AS userid,
    r."ID" AS routeid,
    g."FBleau" AS gradefbleau,
    ur."Status" AS status,
    r."GymID" AS gymid,
    r."SetDate" AS setdate,
    r."RemoveDate" AS removedate,
    r."AdminID" AS adminid
   FROM (((public."Route" r
     JOIN public."Grade" g ON ((r."GradeID" = g."ID")))
     LEFT JOIN public."UserRoute" ur ON ((r."ID" = ur."RouteID")))
     LEFT JOIN public."User" u ON ((u."ID" = ur."UserID")));


ALTER VIEW public.userroutegradeview OWNER TO postgres;

--
-- Name: Admin ID; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Admin" ALTER COLUMN "ID" SET DEFAULT nextval('public."Admin_ID_seq"'::regclass);


--
-- Name: Grade ID; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Grade" ALTER COLUMN "ID" SET DEFAULT nextval('public."Grade_ID_seq"'::regclass);


--
-- Name: Gym ID; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Gym" ALTER COLUMN "ID" SET DEFAULT nextval('public."Gym_ID_seq"'::regclass);


--
-- Name: Route ID; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Route" ALTER COLUMN "ID" SET DEFAULT nextval('public."Route_ID_seq"'::regclass);


--
-- Name: Session ID; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Session" ALTER COLUMN "ID" SET DEFAULT nextval('public."Session_ID_seq"'::regclass);


--
-- Name: User ID; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."User" ALTER COLUMN "ID" SET DEFAULT nextval('public."User_ID_seq"'::regclass);


--
-- Data for Name: Admin; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."Admin" ("ID", "GymID", "UserID") FROM stdin;
1	1	1
2	2	2
3	3	3
4	4	4
5	5	5
\.


--
-- Data for Name: Grade; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."Grade" ("ID", "FBleau", "VScale") FROM stdin;
1	3	VB
2	4	V0
3	5	V1
4	6A	V2
5	6B	V3
6	6C	V4
7	7A	V5
8	7B	V6
9	7C	V7
10	8A	V8
\.


--
-- Data for Name: Gym; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."Gym" ("ID", "Name", "Street", "StreetNumber", "Postcode", "City") FROM stdin;
1	Boulderklub Berlin	Friedrichstraße	45	10117	Berlin
2	Kletterhalle München	Landsberger Straße	100	80339	Munich
3	Stuttgart Bouldergarten	Königstraße	12	70173	Stuttgart
4	Cologne Climbing Center	Rheinauhafen	1	50678	Cologne
5	Hamburg Kletterwelt	Hafenstraße	22	20457	Hamburg
\.


--
-- Data for Name: Route; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."Route" ("ID", "GymID", "GradeID", "SetDate", "RemoveDate", "AdminID") FROM stdin;
4	2	7	2025-03-15	\N	2
5	3	3	2025-04-05	\N	3
7	4	9	2025-05-01	\N	4
8	5	10	2025-05-15	\N	5
9	1	1	2025-02-01	\N	1
10	1	2	2025-02-02	\N	1
11	1	3	2025-02-03	\N	1
12	1	4	2025-02-04	\N	1
13	2	5	2025-02-05	\N	2
14	2	1	2025-02-06	\N	2
15	2	2	2025-02-07	\N	2
16	2	3	2025-02-08	\N	2
17	3	4	2025-02-09	\N	3
18	3	5	2025-02-10	\N	3
19	3	1	2025-02-11	\N	3
20	3	2	2025-02-12	\N	3
21	4	3	2025-02-13	\N	4
22	4	4	2025-02-14	\N	4
23	4	5	2025-02-15	\N	4
24	4	1	2025-02-16	\N	4
25	5	2	2025-02-17	\N	5
26	5	3	2025-02-18	\N	5
27	5	4	2025-02-19	\N	5
28	5	5	2025-02-20	\N	5
\.


--
-- Data for Name: Session; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."Session" ("ID", "UserID", "CustomName", "Date", "Feedback") FROM stdin;
1	1	Morning Bouldering	2025-06-01	Great session, felt strong!
2	2	Training with Friends	2025-06-02	Fun but tiring.
3	3	Project Session	2025-06-03	Worked on my 7A project.
4	4	Evening Climbing	2025-06-04	Good progress on overhangs.
5	5	Weekend Bouldering	2025-06-05	Tried new routes, loved it!
\.


--
-- Data for Name: SessionRoute; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."SessionRoute" ("SessionID", "RouteID", "Tries", "Rating") FROM stdin;
4	7	4	5
5	8	1	5
\.


--
-- Data for Name: User; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."User" ("ID", "Name", "Username", "Mail", "PasswordHash", "Street", "StreetNumber", "Postcode", "City", "Role") FROM stdin;
1	Max Mustermann	maxm	max.mustermann@example.com	secure123	Hauptstraße	10	10115	Berlin	user
2	Anna Schmidt	annas	anna.schmidt@example.com	anna2025	Bahnhofstraße	25	80335	Munich	user
3	Lena Bauer	lenab	lena.bauer@example.com	climber45	Alpenweg	7	70173	Stuttgart	user
4	Tom Weber	tomw	tom.weber@example.com	boulder2025	Waldstraße	15	50667	Cologne	user
5	Lisa Müller	lisam	lisa.mueller@example.com	gymlover	Seeweg	3	20144	Hamburg	user
6	\N	boulderking	boulderking@cphsouth.com	$2a$06$.JoUWsFhMOUsTpWtOPFa.uBggvSGQ8fy5lpcVA084o4DUphgbFoaW	\N	\N	\N	\N	user
7	\N	climber01	climber01@example.com	$2a$06$B5T0aeUPRqVcGPuu2/u5Dem17pg4aG7LI4.w6BY/j.FZ1a.pPc.3q	\N	\N	\N	\N	user
8	\N	routesetter01	routesetter01@example.com	$2a$06$kWX38pzomNiVgEAk3hZxGO1OAk/b2uNzzlNLUUV0uNFxIio92vY/G	\N	\N	\N	\N	admin
\.


--
-- Data for Name: UserRoute; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."UserRoute" ("UserID", "RouteID", "Status") FROM stdin;
1	15	Flash
1	28	Attempted
4	5	Attempted
1	13	Top
1	9	Top
1	4	Attempted
4	4	Attempted
1	7	Attempted
1	10	Top
1	14	Top
1	12	Top
1	8	Attempted
\.


--
-- Name: Admin_ID_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Admin_ID_seq"', 5, true);


--
-- Name: Grade_ID_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Grade_ID_seq"', 10, true);


--
-- Name: Gym_ID_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Gym_ID_seq"', 5, true);


--
-- Name: Route_ID_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Route_ID_seq"', 28, true);


--
-- Name: Session_ID_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Session_ID_seq"', 5, true);


--
-- Name: User_ID_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."User_ID_seq"', 8, true);


--
-- Name: Admin Admin_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Admin"
    ADD CONSTRAINT "Admin_pkey" PRIMARY KEY ("ID");


--
-- Name: Grade Grade_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Grade"
    ADD CONSTRAINT "Grade_pkey" PRIMARY KEY ("ID");


--
-- Name: Gym Gym_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Gym"
    ADD CONSTRAINT "Gym_pkey" PRIMARY KEY ("ID");


--
-- Name: Route Route_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Route"
    ADD CONSTRAINT "Route_pkey" PRIMARY KEY ("ID");


--
-- Name: SessionRoute SessionRoute_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."SessionRoute"
    ADD CONSTRAINT "SessionRoute_pkey" PRIMARY KEY ("SessionID", "RouteID");


--
-- Name: Session Session_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Session"
    ADD CONSTRAINT "Session_pkey" PRIMARY KEY ("ID");


--
-- Name: Admin UniqueAdminPerUser; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Admin"
    ADD CONSTRAINT "UniqueAdminPerUser" UNIQUE ("UserID");


--
-- Name: UserRoute UserRoute_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."UserRoute"
    ADD CONSTRAINT "UserRoute_pkey" PRIMARY KEY ("UserID", "RouteID");


--
-- Name: User User_Mail_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."User"
    ADD CONSTRAINT "User_Mail_key" UNIQUE ("Mail");


--
-- Name: User User_Username_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."User"
    ADD CONSTRAINT "User_Username_key" UNIQUE ("Username");


--
-- Name: User User_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."User"
    ADD CONSTRAINT "User_pkey" PRIMARY KEY ("ID");


--
-- Name: Admin Admin_GymID_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Admin"
    ADD CONSTRAINT "Admin_GymID_fkey" FOREIGN KEY ("GymID") REFERENCES public."Gym"("ID") ON DELETE CASCADE;


--
-- Name: Admin Admin_UserID_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Admin"
    ADD CONSTRAINT "Admin_UserID_fkey" FOREIGN KEY ("UserID") REFERENCES public."User"("ID") ON DELETE CASCADE;


--
-- Name: Route Route_AdminID_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Route"
    ADD CONSTRAINT "Route_AdminID_fkey" FOREIGN KEY ("AdminID") REFERENCES public."Admin"("ID");


--
-- Name: Route Route_GradeID_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Route"
    ADD CONSTRAINT "Route_GradeID_fkey" FOREIGN KEY ("GradeID") REFERENCES public."Grade"("ID");


--
-- Name: Route Route_GymID_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Route"
    ADD CONSTRAINT "Route_GymID_fkey" FOREIGN KEY ("GymID") REFERENCES public."Gym"("ID") ON DELETE CASCADE;


--
-- Name: SessionRoute SessionRoute_RouteID_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."SessionRoute"
    ADD CONSTRAINT "SessionRoute_RouteID_fkey" FOREIGN KEY ("RouteID") REFERENCES public."Route"("ID") ON DELETE CASCADE;


--
-- Name: SessionRoute SessionRoute_SessionID_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."SessionRoute"
    ADD CONSTRAINT "SessionRoute_SessionID_fkey" FOREIGN KEY ("SessionID") REFERENCES public."Session"("ID") ON DELETE CASCADE;


--
-- Name: Session Session_UserID_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Session"
    ADD CONSTRAINT "Session_UserID_fkey" FOREIGN KEY ("UserID") REFERENCES public."User"("ID") ON DELETE CASCADE;


--
-- Name: UserRoute UserRoute_RouteID_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."UserRoute"
    ADD CONSTRAINT "UserRoute_RouteID_fkey" FOREIGN KEY ("RouteID") REFERENCES public."Route"("ID") ON DELETE CASCADE;


--
-- Name: UserRoute UserRoute_UserID_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."UserRoute"
    ADD CONSTRAINT "UserRoute_UserID_fkey" FOREIGN KEY ("UserID") REFERENCES public."User"("ID") ON DELETE CASCADE;


--
-- PostgreSQL database dump complete
--

\unrestrict mtSjt8fDz3KPVpyFGb2d5hurM7ySlapWYnxweAtnwB5iOjLykssQaaeSAtTM1iu

