using Pulse.Models;
using Pulse.Services;

namespace Pulse.Data;

public static class DbSeeder
{
    public static void Seed(AppDbContext db)
    {
        // Krijon bazën dhe tabelat nëse nuk ekzistojnë (fallback ndaj skriptit SQL).
        db.Database.EnsureCreated();

        // --- Kategoritë ---
        if (!db.Categories.Any())
        {
            db.Categories.AddRange(
                new Category { Name = "Bota" },
                new Category { Name = "Rajoni" },
                new Category { Name = "Vendi" },
                new Category { Name = "Sport" },
                new Category { Name = "ShowBiz" });
            db.SaveChanges();
        }

        // --- Admini fillestar (admin / admin123) ---
        if (!db.Admins.Any())
        {
            db.Admins.Add(new Admin
            {
                Username = "admin",
                PasswordHash = PasswordHasher.Hash("admin123")
            });
            db.SaveChanges();
        }

        // --- Lajme shembull ---
        if (!db.News.Any())
        {
            int Cat(string name) => db.Categories.First(c => c.Name == name).Id;

            db.News.AddRange(
                new News
                {
                    Title = "Samiti botëror mbyllet me marrëveshje historike për klimën",
                    Summary = "Liderët botërorë ranë dakord për objektiva të reja të reduktimit të emetimeve deri në vitin 2035.",
                    Content = "Pas tri ditësh negociatash intensive, delegacionet arritën një marrëveshje që konsiderohet kthesë për politikat globale mjedisore. Marrëveshja parasheh fonde të reja për vendet në zhvillim dhe një mekanizëm monitorimi më të rreptë.",
                    ImageUrl = "https://images.unsplash.com/photo-1451187580459-43490279c0fa?w=1200&q=80",
                    PublishedAt = DateTime.Now.AddHours(-2),
                    CategoryId = Cat("Bota")
                },
                new News
                {
                    Title = "Rajoni përballet me valë të re investimesh në energji të gjelbër",
                    Summary = "Projekte të mëdha diellore dhe të erës po ndryshojnë hartën energjetike të rajonit.",
                    Content = "Investitorë ndërkombëtarë kanë shprehur interes për ndërtimin e parqeve të reja diellore. Ekspertët thonë se kjo do të ulë varësinë nga importi i energjisë dhe do të krijojë mijëra vende pune.",
                    ImageUrl = "https://images.unsplash.com/photo-1509391366360-2e959784a276?w=1200&q=80",
                    PublishedAt = DateTime.Now.AddHours(-5),
                    CategoryId = Cat("Rajoni")
                },
                new News
                {
                    Title = "Qeveria prezanton buxhetin e ri me fokus te arsimi dhe shëndetësia",
                    Summary = "Rritje e ndjeshme e fondeve për sektorin publik në propozimin e fundit buxhetor.",
                    Content = "Propozimi i ri buxhetor parasheh rritje të pagave në sektorin e arsimit dhe shëndetësisë, si dhe investime kapitale në infrastrukturë. Opozita ka kërkuar transparencë më të madhe në shpenzime.",
                    ImageUrl = "https://images.unsplash.com/photo-1524995997946-a1c2e315a42f?w=1200&q=80",
                    PublishedAt = DateTime.Now.AddHours(-8),
                    CategoryId = Cat("Vendi")
                },
                new News
                {
                    Title = "Kombëtarja siguron kualifikimin pas një ndeshjeje dramatike",
                    Summary = "Fitorja në minutat e fundit dërgon skuadrën në fazën vijuese të garës.",
                    Content = "Në një atmosferë elektrizuese, ekipi kombëtar realizoi golin vendimtar në kohën shtesë. Trajneri e cilësoi këtë si arritjen më të madhe të viteve të fundit.",
                    ImageUrl = "https://images.unsplash.com/photo-1508098682722-e99c43a406b2?w=1200&q=80",
                    PublishedAt = DateTime.Now.AddHours(-12),
                    CategoryId = Cat("Sport")
                },
                new News
                {
                    Title = "Ylli i muzikës surprizon fansat me album të papritur",
                    Summary = "Publikimi i befasishëm theu rekordet e dëgjueshmërisë brenda pak orësh.",
                    Content = "Pa asnjë paralajmërim, artisti publikoi një album të ri që u bë menjëherë numër një në platformat streaming. Kritikët e kanë vlerësuar si punën më të pjekur të karrierës.",
                    ImageUrl = "https://images.unsplash.com/photo-1493225457124-a3eb161ffa5f?w=1200&q=80",
                    PublishedAt = DateTime.Now.AddDays(-1),
                    CategoryId = Cat("ShowBiz")
                },
                new News
                {
                    Title = "Teknologjia e re e transportit publik nis testimin në kryeqytet",
                    Summary = "Autobusë elektrikë me lidhje digjitale premtojnë udhëtime më të shpejta.",
                    Content = "Projekti pilot përfshin një flotë autobusësh elektrikë me sistem inteligjent të menaxhimit të trafikut. Qytetarët do të mund të ndjekin në kohë reale lëvizjen e linjave përmes një aplikacioni.",
                    ImageUrl = "https://images.unsplash.com/photo-1570125909232-eb263c188f7e?w=1200&q=80",
                    PublishedAt = DateTime.Now.AddDays(-1).AddHours(-3),
                    CategoryId = Cat("Vendi")
                });
            db.SaveChanges();
        }
    }
}
