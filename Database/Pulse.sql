/* =====================================================================
   PULSE — Portal lajmesh (ASP.NET Core 8 MVC / SQL Server)
   Skript i plotë dhe idempotent: drop & create i bazës, tabelave dhe
   të dhënave fillestare. Ekzekuto me:
     sqlcmd -S localhost -E -i Database\Pulse.sql -f 65001
   (opsioni -f 65001 është i domosdoshëm për shkronjat shqipe / UTF-8)
   ===================================================================== */

IF DB_ID('PulseDb') IS NOT NULL
BEGIN
    ALTER DATABASE PulseDb SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE PulseDb;
END
GO

CREATE DATABASE PulseDb;
GO

USE PulseDb;
GO

/* ---------- Tabelat ---------- */
CREATE TABLE Categories (
    Id   INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL UNIQUE
);
GO

CREATE TABLE News (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    Title       NVARCHAR(200) NOT NULL,
    Summary     NVARCHAR(400) NULL,
    Content     NVARCHAR(MAX) NOT NULL,
    ImageUrl    NVARCHAR(300) NULL,
    PublishedAt DATETIME       NOT NULL DEFAULT GETDATE(),
    CategoryId  INT            NOT NULL,
    CONSTRAINT FK_News_Categories FOREIGN KEY (CategoryId)
        REFERENCES Categories(Id)
);
GO

CREATE TABLE Admins (
    Id           INT IDENTITY(1,1) PRIMARY KEY,
    Username     NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(64) NOT NULL
);
GO

/* ---------- Kategoritë ---------- */
INSERT INTO Categories (Name) VALUES
    (N'Bota'), (N'Rajoni'), (N'Vendi'), (N'Sport'), (N'ShowBiz');
GO

/* ---------- Admini fillestar (admin / admin123, SHA-256 hex) ---------- */
INSERT INTO Admins (Username, PasswordHash) VALUES
    (N'admin', N'240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9');
GO

/* ---------- Lajme shembull ---------- */
DECLARE @Bota INT    = (SELECT Id FROM Categories WHERE Name = N'Bota');
DECLARE @Rajoni INT  = (SELECT Id FROM Categories WHERE Name = N'Rajoni');
DECLARE @Vendi INT   = (SELECT Id FROM Categories WHERE Name = N'Vendi');
DECLARE @Sport INT   = (SELECT Id FROM Categories WHERE Name = N'Sport');
DECLARE @ShowBiz INT = (SELECT Id FROM Categories WHERE Name = N'ShowBiz');

INSERT INTO News (Title, Summary, Content, ImageUrl, PublishedAt, CategoryId) VALUES
(N'Samiti botëror mbyllet me marrëveshje historike për klimën',
 N'Liderët botërorë ranë dakord për objektiva të reja të reduktimit të emetimeve deri në vitin 2035.',
 N'Pas tri ditësh negociatash intensive, delegacionet arritën një marrëveshje që konsiderohet kthesë për politikat globale mjedisore. Marrëveshja parasheh fonde të reja për vendet në zhvillim dhe një mekanizëm monitorimi më të rreptë.',
 N'https://images.unsplash.com/photo-1451187580459-43490279c0fa?w=1200&q=80',
 DATEADD(HOUR, -2, GETDATE()), @Bota),

(N'Rajoni përballet me valë të re investimesh në energji të gjelbër',
 N'Projekte të mëdha diellore dhe të erës po ndryshojnë hartën energjetike të rajonit.',
 N'Investitorë ndërkombëtarë kanë shprehur interes për ndërtimin e parqeve të reja diellore. Ekspertët thonë se kjo do të ulë varësinë nga importi i energjisë dhe do të krijojë mijëra vende pune.',
 N'https://images.unsplash.com/photo-1509391366360-2e959784a276?w=1200&q=80',
 DATEADD(HOUR, -5, GETDATE()), @Rajoni),

(N'Qeveria prezanton buxhetin e ri me fokus te arsimi dhe shëndetësia',
 N'Rritje e ndjeshme e fondeve për sektorin publik në propozimin e fundit buxhetor.',
 N'Propozimi i ri buxhetor parasheh rritje të pagave në sektorin e arsimit dhe shëndetësisë, si dhe investime kapitale në infrastrukturë. Opozita ka kërkuar transparencë më të madhe në shpenzime.',
 N'https://images.unsplash.com/photo-1524995997946-a1c2e315a42f?w=1200&q=80',
 DATEADD(HOUR, -8, GETDATE()), @Vendi),

(N'Kombëtarja siguron kualifikimin pas një ndeshjeje dramatike',
 N'Fitorja në minutat e fundit dërgon skuadrën në fazën vijuese të garës.',
 N'Në një atmosferë elektrizuese, ekipi kombëtar realizoi golin vendimtar në kohën shtesë. Trajneri e cilësoi këtë si arritjen më të madhe të viteve të fundit.',
 N'https://images.unsplash.com/photo-1508098682722-e99c43a406b2?w=1200&q=80',
 DATEADD(HOUR, -12, GETDATE()), @Sport),

(N'Ylli i muzikës surprizon fansat me album të papritur',
 N'Publikimi i befasishëm theu rekordet e dëgjueshmërisë brenda pak orësh.',
 N'Pa asnjë paralajmërim, artisti publikoi një album të ri që u bë menjëherë numër një në platformat streaming. Kritikët e kanë vlerësuar si punën më të pjekur të karrierës.',
 N'https://images.unsplash.com/photo-1493225457124-a3eb161ffa5f?w=1200&q=80',
 DATEADD(DAY, -1, GETDATE()), @ShowBiz),

(N'Teknologjia e re e transportit publik nis testimin në kryeqytet',
 N'Autobusë elektrikë me lidhje digjitale premtojnë udhëtime më të shpejta.',
 N'Projekti pilot përfshin një flotë autobusësh elektrikë me sistem inteligjent të menaxhimit të trafikut. Qytetarët do të mund të ndjekin në kohë reale lëvizjen e linjave përmes një aplikacioni.',
 N'https://images.unsplash.com/photo-1570125909232-eb263c188f7e?w=1200&q=80',
 DATEADD(HOUR, -27, GETDATE()), @Vendi);
GO

PRINT 'PulseDb u krijua me sukses.';
GO
