
using Microsoft.AspNetCore.Mvc;
using mvc4.Models;
using SelectPdf;
using System.Diagnostics;
using System.Reflection.Metadata;
using SelectPdf;
using HtmlToPdf = SelectPdf.HtmlToPdf;
using mvc4.Data;
using Microsoft.Net.Http.Headers;
using System.Text;

namespace mvc4.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;
        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}

        public readonly AppDbContext _context;
        public HomeController(AppDbContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            List<InvestorModel> users = new List<InvestorModel>();
            users = _context.InvestorTable.ToList();
            return View(users);
        }

        //print pdf of Investors form
        public IActionResult Demo(int id){
            InvestorModel? invMod = _context.InvestorTable.Where(inv => inv.InvestorId == id).FirstOrDefault();
            return View(invMod);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // view a single investors details
        public IActionResult InvestorView(int id)
        {
            InvestorModel? invMod = _context.InvestorTable.Where(inv => inv.InvestorId == id).FirstOrDefault();
            return View(invMod);
        }

        public IActionResult EditView(int id)
        {
            InvestorModel? investor = _context.InvestorTable.FirstOrDefault(inv => inv.InvestorId == id);
            return View(investor);
        }

        [HttpPost]
        public IActionResult EditInvestor(InvestorModel investor)
        {
            InvestorModel? inves = _context.InvestorTable.FirstOrDefault(u => u.InvestorId == investor.InvestorId);
            investor.InvestorPassword = inves.InvestorPassword;
            investor.InvestorLogo = inves.InvestorLogo;
            investor.InvestorActive = inves.InvestorActive;

            _context.InvestorTable.Remove(inves);
            _context.InvestorTable.Add(investor);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult DeleteInvestor(int id)
        {
            try
            {
                InvestorModel? investor = _context.InvestorTable.FirstOrDefault(inv => inv.InvestorId == id);
                if (investor != null)
                {
                    _context.InvestorTable.Remove(investor);
                }
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public IActionResult ExportCSV()
        {
            List<InvestorModel> invM = _context.InvestorTable.ToList<InvestorModel>();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Id,Name,Address,Email,Contact,Date,Active");
            foreach (var inv in invM)
            {
                sb.Append(inv.InvestorId.ToString() + ',');
                sb.Append(inv.InvestorName + ',');
                sb.Append(inv.InvestorAddress + ',');
                sb.Append(inv.InvestorEmail + ',');
                sb.Append(inv.InvestorContact + ',');
                sb.Append(inv.InvestorDate.ToString() + ',');
                sb.Append(inv.InvestorActive.ToString() + ',');
                sb.AppendLine();
            }
            return File(Encoding.UTF8.GetBytes(sb.ToString()),"text/csv","Investor.csv");
        }

        // pdf using html string
        [HttpPost]
        public IActionResult ExportPDF(string html)
        {
            //string tHtml = html.Replace("StrTag", "<").Replace("EndTag", ">");
            //html = tHtml;
            //HtmlToPdf htp = new HtmlToPdf();
            //SelectPdf.PdfDocument pdfD = htp.ConvertHtmlString(html);
            //byte[] pdf = pdfD.Save();
            //pdfD.Close();

            //return File(pdf,"application/pdf","StudentList.pdf");

            HtmlToPdf converter = new HtmlToPdf();
            PdfDocument doc = converter.ConvertHtmlString(html, "http://localhost:5070");
            return new FileContentResult(doc.Save(), new MediaTypeHeaderValue("application/pdf"))
            {
                FileDownloadName = "Data.pdf"
            };
        }

        // pdf using url
        public IActionResult PrintPDF(string url){
            try{
                var converter = new HtmlToPdf();
                //converter.Options.PdfPageSize =PdfPageSize.A4;
                var doc = converter.ConvertUrl(url);
                return new FileContentResult(doc.Save(), new MediaTypeHeaderValue("application/pdf")){
                    FileDownloadName = "Data.pdf"
                };
            }
            catch(Exception e){
                Console.Write(e);
                return View();
            }
        }

    }
}
