using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Xceed.Words.NET;
using Xceed.Document.NET;
using Airclub.Models;
using System.Data.Entity;

namespace Airclub.Controllers
{
    public class ReportingController : Controller
    {
        AircraftContext db = new AircraftContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Contract(int? id)
        {
            if (!User.Identity.IsAuthenticated) return Redirect("/Account/Login");
            Contract contract = db.Contracts
                .Include(a => a.Service)
                .Include(a => a.Administrator)
                .Include(a => a.Customer)
                .Include(a => a.Service.Aircraft)
                .Include(a => a.Service.Aircraft.Base)
                .First(a => a.Id == id);
            using (MemoryStream ms = new MemoryStream())
            {
                string ContractID = contract.Id.ToString();
                string nameClient = contract.Customer.Name;
                string nameStaff = contract.Administrator.Name;
                string RegistrationNum = contract.Service.Aircraft.RegistrationNumber;
                string Manufacturer = contract.Service.Aircraft.Manufacturer;
                string Service = contract.Service.Name;
                DateTime DateOfSigning = contract.ContractSignDate;
                DateTime DateOfStart = contract.ContractStartDate;
                DateTime DateOfCompletion = contract.ContractFinishDate;
                string sum = contract.TotalCost.ToString();
                string cost = contract.Service.Price.ToString();
                string PostIndex = contract.Service.Aircraft.Base.PostIndex;
                string City = contract.Service.Aircraft.Base.City;
                string Address = contract.Service.Aircraft.Base.Adress;

                // создаём документ
                DocX document = DocX.Create(ms);
                document.SetDefaultFont(new Xceed.Document.NET.Font("Times New Roman"), 12);
                document.MarginLeft = 85;
                document.MarginTop = 56.7f;
                document.MarginBottom = 56.7f;
                document.MarginRight = 42.5f;
                // Вставляем параграф и указываем текст
                Paragraph text = document.InsertParagraph();
                document.InsertParagraph($"Клієнтський договір № {ContractID}").Bold().
                        Alignment = Alignment.center;
                document.InsertParagraph($"Місто {City}\t\t\t\t\t\t\t\t{DateOfSigning.Date.ToString().Substring(0, 10)} рік");
                // вставляем параграф и добавляем текст
                Paragraph firstParagraph = document.InsertParagraph();
                // выравниваем параграф по правой стороне
                firstParagraph.Alignment = Alignment.both;
                firstParagraph.AppendLine("Авіа-клуб «Student Aero», що являється платником податку на загальних підставах " +
                    $"в особі {nameClient} (надалі іменується Замовник) з однієї сторони, та Авіа-клуб «Student Aero» " +
                    $"(надалі іменується Виконавець) в особі {nameStaff}, діючого на підставі  " +
                    "№20037808 від 01.11.2018, з іншої сторони, заключили даний Договір про нижче наведене:");
                document.InsertParagraph("\n1. Предмет договору.").Bold().Alignment = Alignment.center;
                document.InsertParagraph("1.1. За даним Договором Виконавець за плату та за завданням Замовника " +
                    "зобов’язується надати комплекс послуг з організації спортивно-розважального заходу, а саме заходу на " +
                    $"повітряному судні.").Alignment = Alignment.both;
                //document.InsertParagraph("\n2. Права та обов’язки сторін.").Bold().Alignment = Alignment.center;
                document.InsertParagraph("\n2. Права та обов’язки сторін.").Bold().Alignment = Alignment.center;
                document.InsertParagraph("2.1. Для виконання даного Договору Виконавець зобов’язується організувати " +
                    $"спортивно-розважальні послуги на повітряному судні «{RegistrationNum} {Manufacturer}» {DateOfStart.Date.ToString().Substring(0, 10)} року " +
$"до {DateOfCompletion.Date.ToString().Substring(0, 10)} року " +
                    $"в місці зустрічі на авіабазі {PostIndex} м. {City}, {Address}").Alignment = Alignment.both;
                document.InsertParagraph("2.2.Для виконання даного Договору Замовник зобов’язується:\n" +
                    "2.2.1. Під час здійснення прогулянкового польоту не допускати використання особами, " +
                    "що знаходяться на судні під відповідальність Замовника(своїми клієнтами, співробітниками " +
                    "та іншими залученими їм особами), судна в цілях, що не відповідають призначенню поїздки та / " +
                    "або несуть ризик псування або пошкодження судна та / або майна, що на ньому знаходиться;").Alignment = Alignment.both;
                document.InsertParagraph("2.2.2. Виконувати та забезпечити виконання особами, що знаходяться на судні " +
                    "під відповідальність Замовника, правил пожежної безпеки, охорони праці, норм і правил санітарії, " +
                    "інших загальнообов’язкових правил та норм поведінки, а також спеціальних (внутрішніх) вимог " +
                    "безпеки під час перебування на даному судні, так само як і законних, розумних та обґрунтованих " +
                    "вимог старшого на судні;\n" +
                    "2.2.3. Про всі інциденти та / або нестандартні ситуації, та / або надзвичайні події, " +
                    "що загрожують життю та здоров’ю людей і збереженню майна, в тому числі третіх осіб, " +
                    "негайно інформувати старшого на судні та представника Виконавця, а також приймати посильні " +
                    "заходи для їх попередження, не піддаючи себе при цьому ризику.\n" +
                    "2.3. Сторони мають право вимагати одна від одної добросовісного виконання прийнятих зобов’язань.\n").Alignment = Alignment.both;
                document.InsertParagraph("3. Вартість послуг та порядок розрахунків.").Bold().Alignment = Alignment.center;
                document.InsertParagraph("3.1. Загальна вартість послуг, що надаються згідно даного " +
                    $"договору становить: {sum} грн., в тому числі: {Service} становить {cost} грн.").Alignment = Alignment.both;
                document.InsertParagraph("3.2. Розрахунки між сторонами здійснюються в усній формі.\n").Alignment = Alignment.both;
                document.InsertParagraph("4. Відповідальність сторін та порядок вирішення суперечок.").Bold().Alignment = Alignment.center;
                document.InsertParagraph("4.1. За порушення зобов’язань, що витікають з умов даного Договору, " +
                    "сторони несуть відповідальність, передбачену чинним законодавством України. При цьому винна " +
                    "Сторона зобов’язана відшкодувати іншій Стороні завдані цим збитки.\n" +
                    "4.2. Усі суперечності та спори, що виникли з даного Договору чи в зв’язку з ним, " +
                    "вирішуються шляхом проведення переговорів представниками сторін. У випадку неможливості " +
                    "досягнення згоди суперечка передається на розгляд до суду у відповідності з правилами " +
                    "підвідомчості і підсудності, встановленими чинним законодавством України. Дотримання " +
                    "досудового(претензійного) порядку врегулювання спорів являється для Сторін обов’язковим.\n" +
                    "4.3. Замовник несе майнову відповідальність за шкоду, завдану ним або іншими особами на судні " +
                    "під відповідальність Замовника, майну Виконавця, або третіх осіб.\n" +
                    "4.4. У випадку порушення Замовником п. 3.2 Договору та зриву(відміни) в зв’язку з цим " +
                    "спортивно-розважального заходу на повітряному судні, вартість бронювання спортивно-розважального заходу на " +
                    "повітряному судні згідно п.2.1 не повертається. Можливість подальшого виконання Договору " +
"обговорюється Сторонами шляхом укладання Додаткового погодження до Договору.\n").Alignment = Alignment.both;
                document.InsertParagraph("5. Термін дії, обставини форс-мажор та інші умови Договору.").Bold().Alignment = Alignment.center;
                document.InsertParagraph("5.1. Даний договір набуває чинності з моменту початку спортивно-розважального заходу та діє до " +
                    "моменту його завершення.\n" +
                    "5.2. Одностороння зміна умов або відмова від Договору не допускається.\n" +
                    "5.3. Сторони звільняються від відповідальності за порушення Договору у випадку дії обставин " +
                    "непереборної сили(форс - мажор), які трапилися не по їх волі та перешкоджають виконанню " +
                    "Договору. Такими обставинами є: війна та військові дії, повстання, страйки, локаути, масові " +
                    "заворушення, ембарго, обмеження подачі електроенергії та енергоносіїв, епідемії, пожежі, " +
                    "вибухи, природні катастрофи, дорожні пригоди.\n" +
                    "5.4. Даний Договір складений і укладений при повному розумінні Сторонами його умов " +
                    "українською мовою на двох сторінках у двох примірниках рівної юридичної сили по одному для " +
                    "кожної із Сторін.\n\n\n").Alignment = Alignment.both;
                document.InsertParagraph("______________/__________________      ______________/__________________").Alignment = Alignment.both;
                document.InsertParagraph("М.П.                        М.П. ").Alignment = Alignment.both;
                document.InsertParagraph($"\n\n\n\n\nДата друку: {DateTime.Today.ToString().Substring(0, 10)}").Alignment = Alignment.right;
                document.Save();

                Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                Response.AddHeader("content-disposition",
                "attachment;filename=Contract №" + ContractID + ".docx");
                Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
            }

            return null;

            /*if (!User.Identity.IsAuthenticated) return Redirect("/Account/Login");
            if (id == null)
            {
                return HttpNotFound();
            }
            Contract contract = db.Contracts
                .Include(a => a.Service)
                .Include(a => a.Administrator)
                .Include(a => a.Customer)
                .Include(a => a.Service.Aircraft)
                .Include(a => a.Service.Aircraft.Base)
                .First(a => a.Id == id);
            string ttf = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "ARIAL.TTF");
            var baseFont = BaseFont.CreateFont(ttf, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            var font = new iTextSharp.text.Font(baseFont, iTextSharp.text.Font.DEFAULTSIZE, iTextSharp.text.Font.NORMAL);
            using (MemoryStream ms = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 25, 25, 30, 30);

                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                document.Open();

                document.Add(new Paragraph("Акт наданих послуг", font));
                document.Add(new Paragraph("до договору від від " + contract.ContractSignDate + " №" + contract.Id, font));
                document.Add(new Paragraph("м. " + contract.Service.Aircraft.Base.City + "                    " + contract.ContractSignDate, font));
                document.Add(new Paragraph("Сторона, що надає послуги, ООО\"Студентавиклуб\" в особі адміністратора " + contract.Administrator.Name + ", який діє на підставі статусу, та сторона, що отримує послуги, фізичне лицо " + contract.Customer.Name + ", склали цей Акт про те, що послуги з найменуванням \"" + contract.Service.Name + "\" одиниці авіатехніки класу \"" + contract.Service.Aircraft.Class + "\", моделі " + contract.Service.Aircraft.Model + ", реєстраційний номер " + contract.Service.Aircraft.RegistrationNumber + " для часного використання надані в повному обсязі відповідно до Договору.", font));
                document.Add(new Paragraph("Загальна вартість послуг, наданих у період із " + contract.ContractStartDate + " по " + contract.ContractFinishDate + ", становить " + contract.TotalCost + " грн.", font));
                document.Add(new Paragraph("Сторони претензій одна до одної не мають.", font));
                document.Add(new Paragraph("Цей Акт складено у двох примірниках, що мають однакову силу, по одному для кожної зі Сторін.", font));
                document.Add(new Paragraph("Сторона, що надає послуги:____________________________                                                Сторона, що отримує послуги:__________________________", font));
                document.Add(new Paragraph("Дата:" + contract.ContractSignDate, font));

                document.Close();
                writer.Close();
                Response.ContentType = "pdf/application";
                Response.AddHeader("content-disposition",
                "attachment;filename=Contract" + id + ".pdf");
                Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
            }
            return null;*/
        }

        public ActionResult Aircrafts()
        {
            if (!User.Identity.IsAuthenticated) return Redirect("/Account/Login");
            IEnumerable<Base> bases = db.Bases;
            using (MemoryStream ms = new MemoryStream())
            {
                DocX document = DocX.Create(ms);
                document.SetDefaultFont(new Xceed.Document.NET.Font("Times New Roman"), 12);
                document.MarginLeft = 85;
                document.MarginTop = 56.7f;
                document.MarginBottom = 56.7f;
                document.MarginRight = 42.5f;

                document.InsertParagraph("Розподілення яхт за портами\n").Bold().Alignment = Alignment.center;
                foreach (Base airbase in bases)
                {
                    IEnumerable<Aircraft> aircrafts = db.Aircrafts.Include(m => m.Base).Where(m => m.BaseId == airbase.Id);
                    if (aircrafts.Count() == 0)
                        continue;
                    document.InsertParagraph($"Індекс: {airbase.PostIndex}").Alignment = Alignment.center;
                    document.InsertParagraph($"Місто: {airbase.City}").Alignment = Alignment.center;
                    document.InsertParagraph($"Адреса: {airbase.Adress}").Alignment = Alignment.center;
                    Table table = document.AddTable(aircrafts.Count() + 1, 6);
                    table.Alignment = Alignment.center;
                    table.Design = TableDesign.TableGrid;
                    string[] names = { "Реєстраційний номер", "Виробник", "Модель", "Клас", "Рік виробництва", "Статус" };
                    for (int k = 0; k < names.Length; ++k)
                    {
                        table.Rows[0].Cells[k].Paragraphs[0].Append(names[k]).Bold().Alignment = Alignment.center;
                    }
                    int j = 0;
                    foreach (Aircraft aircraft in aircrafts)
                    {
                        table.Rows[j + 1].Cells[0].Paragraphs[0].Append(aircraft.RegistrationNumber).Alignment = Alignment.center;
                        table.Rows[j + 1].Cells[1].Paragraphs[0].Append(aircraft.Manufacturer).Alignment = Alignment.center;
                        table.Rows[j + 1].Cells[2].Paragraphs[0].Append(aircraft.Model).Alignment = Alignment.center;
                        table.Rows[j + 1].Cells[3].Paragraphs[0].Append(aircraft.Class).Alignment = Alignment.center;
                        table.Rows[j + 1].Cells[4].Paragraphs[0].Append(aircraft.YearOfProduction.ToString()).Alignment = Alignment.center;
                        table.Rows[j + 1].Cells[5].Paragraphs[0].Append(aircraft.Status).Alignment = Alignment.center;
                        j++;
                    }
                    document.InsertParagraph().InsertTableAfterSelf(table);
                    document.InsertParagraph();
                }
                document.InsertParagraph($"\n\n\n Дата формування: {DateTime.Now}").Alignment = Alignment.right;
                document.Save();

                Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                Response.AddHeader("content-disposition",
                "attachment;filename=Aircrafts.docx");
                Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
            }
            return null;
        }
    }
}