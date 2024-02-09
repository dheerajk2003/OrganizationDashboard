
using Microsoft.AspNetCore.Mvc;
using mvc4.Models;
using SelectPdf;
using System.Diagnostics;
using System.Collections.Generic;
using System.Reflection.Metadata;
using SelectPdf;
using HtmlToPdf = SelectPdf.HtmlToPdf;
using mvc4.Data;
using Microsoft.Net.Http.Headers;
using System.Text;
using ZXing;
using ZXing.QrCode;
using ZXing.Common;
using ZXing.Windows.Compatibility;
using System.Drawing;
using mvc4.ViewModels;
using Humanizer;

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
            //List<InvestorModel> users = new List<InvestorModel>();
            //users = _context.InvestorTable.ToList();
            //return View(users);

            var myData = from i in _context.InvestorTable
                         join f in _context.FundTable on i.FundId equals f.FundId 
                         join c in _context.ClientTable on i.ClientId equals c.ClientId
                         select new JoinViewModel { Investor = i, Fund = f, Client = c };
            return View(myData);
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
                return RedirectToAction("Index");
            }
        }

        public IActionResult ExportCSV()
        {
            List<InvestorModel> invM = _context.InvestorTable.ToList<InvestorModel>();
            StringBuilder sb = new StringBuilder();
            foreach (var inv in invM)
            {
                sb.Append(inv.InvestorId.ToString() + ',');
                sb.Append(inv.InvestorName + ',');
                sb.Append(inv.InvestorAddress + ',');
                sb.Append(inv.InvestorEmail + ',');
                sb.Append(inv.InvestorContact + ',');
                sb.Append(inv.InvestorLogo + ',');
                sb.Append(inv.InvestorContactA + ',');
                sb.Append(inv.InvestorDate.ToString() + ',');
                sb.Append(inv.InvestorActive.ToString() + ',');
                sb.Append(inv.FundId.ToString() + ',');
                sb.Append(inv.ClientId.ToString() + ',');
                sb.Append("\n");
            }
            return File(Encoding.UTF8.GetBytes(sb.ToString()),"text/csv","Investor.csv");
        }

        // pdf using html string
        [HttpPost]
        public IActionResult ExportPDF(string html)
        {   
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

        public IActionResult InvestorReg()
        {
            var Clients = _context.ClientTable.Select(u => new ClientModel { ClientId = u.ClientId, ClientName = u.ClientName }).ToList();
            var Funds = _context.FundTable.Select(u => new FundModel { FundId = u.FundId, FundName = u.FundName }).ToList();

            //List<List<object>> tList = new List<List<object>>();
            //tList.Add((List<object>)Clients.Cast<object>());
            //tList.Add((List<object>)Funds.Cast<object>());

            IdListModel idm = new IdListModel();
            idm.Fm = Funds;
            idm.Cm = Clients;

            return View(idm);
        }
        [HttpPost]
        public IActionResult AddInvestor(InvestorModel model, IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                string fname = Guid.NewGuid().ToString() + file.FileName;
                string fpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images", fname);
                using (var fileStream = new FileStream(fpath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
                model.InvestorLogo = fname;
            }
            else
            {
                model.InvestorLogo = "photo.png";
            }
            model.InvestorActive = 1;
            _context.InvestorTable.Add(model);
            _context.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult PostFromCsv(IFormFile file)
        {
            string path = string.Empty;
            try
            {
                if (file != null && file.Length > 0)
                {
                    string fname = (DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + file.FileName);
                    path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Csvs", fname);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                }
                if(path != "")
                {
                    var csv = System.IO.File.ReadAllText(path);
                    if(csv != null)
                    {
                        int i = 0;
                        foreach(string row in csv.Split("\n")){
                            i++;
                            if (i == 1) continue;
                            var item = row.Split(',');

                            //sb.Append(inv.InvestorId.ToString() + ',');
                            //sb.Append(inv.InvestorName + ',');
                            //sb.Append(inv.InvestorAddress + ',');
                            //sb.Append(inv.InvestorEmail + ',');
                            //sb.Append(inv.InvestorContact + ',');
                            //sb.Append(inv.InvestorLogo + ',');
                            //sb.Append(inv.InvestorContactA + ',');
                            //sb.Append(inv.InvestorDate.ToString() + ',');
                            //sb.Append(inv.InvestorActive.ToString() + ',');
                            //sb.Append(inv.FundId.ToString() + ',');
                            //sb.Append(inv.ClientId.ToString() + ',');


                            InvestorModel investor = new InvestorModel
                            {
                                //InvestorId = Convert.ToInt32(item[0]),
                                InvestorName = item[1],
                                InvestorAddress = item[2],
                                InvestorEmail = item[3],
                                //InvestorPassword = item[4],
                                InvestorContact = item[4],
                                InvestorLogo = item[5],
                                InvestorContactA = item[6],
                                InvestorDate = DateOnly.Parse(item[7]),
                                InvestorActive = Convert.ToInt32(item[8]),
                                FundId = Convert.ToInt32(item[9]),
                                ClientId = Convert.ToInt32(item[10])
                            };
                            _context.InvestorTable.Add(investor);
                            _context.SaveChanges();
                        }
                    }
                }
            }
            catch(Exception ex) { }
            return RedirectToAction("Index");
        }

        public IActionResult MyAction()
        {
            var myData = from i in _context.InvestorTable
                         join f in _context.FundTable on i.FundId equals f.FundId into fi
                         from f in fi.DefaultIfEmpty()
                         select  new JoinViewModel { Investor = i , Fund = f };
            return View(myData);
        }
    }
}
