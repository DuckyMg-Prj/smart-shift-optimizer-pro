using SmartShift.Core.Model.DTOs;
using SmartShift.Core.Model.DTOs.Users.Shift;
using SmartShift.Core.Model.Source;
using SmartShift.Core.Service.Abstraction;
using SmartShift.Ui.Api.Resourcess;
using System;
using System.Linq;
using System.Security.Claims;
using System.Web.Http;

namespace SmartShift.Ui.Api.Controllers
{
    using iTextSharp.text;
    using iTextSharp.text.pdf;
    using SmartShift.Core.Service.Abstraction;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading;

    [Authorize]
    [RoutePrefix("api/reports")]
    public class ReportsController : ApiController
    {
        private readonly Lazy<IShiftReportService> _reportService;
        public ReportsController(Lazy<IShiftReportService> reportService)
        {
            _reportService = reportService;
        }
        private IShiftReportService reportService => _reportService.Value;
        private int GetUserId()
        {
            var idClaim = ((ClaimsPrincipal)User).Claims.FirstOrDefault(c => c.Type == "id");
            return idClaim != null ? int.Parse(idClaim.Value) : 0;
        }

        [HttpGet, Route("company")]
        public IHttpActionResult Company(int companyId, DateTime fromUtc, DateTime toUtc)
        {
            try
            {
                var result = reportService.GetCompanyReport(companyId, fromUtc, toUtc);
                return Ok(result);
            }
            catch (CustomException ex)
            {
                return Content(ex.Level == ExceptionLevel.Warning
                    ? System.Net.HttpStatusCode.BadRequest
                    : System.Net.HttpStatusCode.InternalServerError,
                    new { message = ex.Message });
            }
        }

        [HttpGet, Route("employee")]
        public IHttpActionResult Employee(DateTime fromUtc, DateTime toUtc)
        {
            var userId = GetUserId();
            var result = reportService.GetEmployeeReport(userId, fromUtc, toUtc);
            return Ok(result);
        }
        [HttpGet, Route("company/exportCsv")]
        public HttpResponseMessage ExportCompanyCsv(int companyId, DateTime fromUtc, DateTime toUtc)
        {
            var report = reportService.GetCompanyReport(companyId, fromUtc, toUtc);

            // Detect language for headers
            var lang = System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
            string[] headers;

            if (lang == "ja")
            {
                headers = new[] { "ユーザーID", "名前", "勤務シフト数", "勤務時間", "残業時間", "休暇日数" };
            }
            else
            {
                headers = new[] { "UserId", "UserName", "TotalShifts", "TotalHours", "OvertimeHours", "OffDays" };
            }

            var csv = new StringBuilder();
            csv.AppendLine(string.Join(",", headers));

            foreach (var e in report.EmployeeSummaries)
            {
                csv.AppendLine($"{e.UserId},{e.UserName},{e.TotalShifts},{e.TotalHours},{e.OvertimeHours},{e.OffDays}");
            }

            var result = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent(csv.ToString(), Encoding.UTF8, "text/csv")
            };

            result.Content.Headers.ContentDisposition =
                new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    FileName = $"CompanyReport_{companyId}_{DateTime.UtcNow:yyyyMMdd}.csv"
                };

            return result;
        }

        [HttpGet, Route("company/exportPdf")]
        public HttpResponseMessage ExportCompanyPdf(int companyId, DateTime fromUtc, DateTime toUtc)
        {
            var report = reportService.GetCompanyReport(companyId, fromUtc, toUtc);

            using (var ms = new MemoryStream())
            {
                // Create PDF document
                var doc = new Document(PageSize.A4, 40, 40, 40, 40);
                PdfWriter.GetInstance(doc, ms);
                doc.Open();

                string[] headers;
                if (Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "ja")
                {
                    string fontPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Font/msgothic.ttc");
                    BaseFont jpBaseFont = BaseFont.CreateFont(fontPath + ",0",BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    Font jpFont = new Font(jpBaseFont, 11);

                    doc.Add(new Paragraph($"会社レポート: {report.CompanyName}", jpFont));
                    doc.Add(new Paragraph($"期間: {report.PeriodStart:yyyy-MM-dd} ～ {report.PeriodEnd:yyyy-MM-dd}", jpFont));
                    doc.Add(new Paragraph("\n", jpFont));

                    var table = new PdfPTable(6) { WidthPercentage = 100 };
                    headers = new[] { "ユーザーID", "名前", "勤務シフト数", "勤務時間", "残業時間", "休暇日数" };
                    foreach (var h in headers)
                    {
                        var headerCell = new PdfPCell(new Phrase(h, jpFont))
                        {
                            BackgroundColor = BaseColor.LIGHT_GRAY,
                            HorizontalAlignment = Element.ALIGN_CENTER
                        };
                        table.AddCell(headerCell);
                    }

                    foreach (var e in report.EmployeeSummaries)
                    {
                        table.AddCell(new Phrase(e.UserId.ToString(), jpFont));
                        table.AddCell(new Phrase(e.UserName, jpFont));
                        table.AddCell(new Phrase(e.TotalShifts.ToString(), jpFont));
                        table.AddCell(new Phrase(e.TotalHours.ToString("F1"), jpFont));
                        table.AddCell(new Phrase(e.OvertimeHours.ToString("F1"), jpFont));
                        table.AddCell(new Phrase(e.OffDays.ToString(), jpFont));
                    }

                    doc.Add(table);
                    doc.Close();

                    var result = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new ByteArrayContent(ms.ToArray())
                    };
                    result.Content.Headers.ContentType =
                        new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
                    result.Content.Headers.ContentDisposition =
                        new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                        {
                            FileName = $"CompanyReport_{companyId}_{DateTime.UtcNow:yyyyMMdd}.pdf"
                        };
                    return result;

                }
                else
                {

                    var titleFont = FontFactory.GetFont("Arial", 16, Font.BOLD);
                    var headerFont = FontFactory.GetFont("Arial", 12, Font.BOLD);
                    var cellFont = FontFactory.GetFont("Arial", 10, Font.NORMAL);

                    doc.Add(new Paragraph($"Company Report: {report.CompanyName}", titleFont));
                    doc.Add(new Paragraph($"Period: {report.PeriodStart:yyyy-MM-dd} → {report.PeriodEnd:yyyy-MM-dd}", cellFont));
                    doc.Add(new Paragraph("\n"));

                    var table = new PdfPTable(6) { WidthPercentage = 100 };

                    headers = new[] { "User ID", "Name", "Total Shifts", "Total Hours", "Overtime Hours", "Off Days" };
                    foreach (var h in headers)
                    {
                        var headerCell = new PdfPCell(new Phrase(h, headerFont))
                        {
                            BackgroundColor = BaseColor.LIGHT_GRAY,
                            HorizontalAlignment = Element.ALIGN_CENTER
                        };
                        table.AddCell(headerCell);
                    }

                    foreach (var e in report.EmployeeSummaries)
                    {
                        table.AddCell(new Phrase(e.UserId.ToString(), cellFont));
                        table.AddCell(new Phrase(e.UserName, cellFont));
                        table.AddCell(new Phrase(e.TotalShifts.ToString(), cellFont));
                        table.AddCell(new Phrase(e.TotalHours.ToString("F1"), cellFont));
                        table.AddCell(new Phrase(e.OvertimeHours.ToString("F1"), cellFont));
                        table.AddCell(new Phrase(e.OffDays.ToString(), cellFont));
                    }

                    doc.Add(table);
                    doc.Close();

                    var result = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new ByteArrayContent(ms.ToArray())
                    };
                    result.Content.Headers.ContentType =
                        new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
                    result.Content.Headers.ContentDisposition =
                        new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                        {
                            FileName = $"CompanyReport_{companyId}_{DateTime.UtcNow:yyyyMMdd}.pdf"
                        };
                    return result;
                }


            }
        }
    }
}
