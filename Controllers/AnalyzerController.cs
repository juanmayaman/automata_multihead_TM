using Microsoft.AspNetCore.Mvc;
using TM_MULTIHEAD_PHISHING_DETECTOR.Models;

namespace TM_MULTIHEAD_PHISHING_DETECTOR.Controllers
{
    public class AnalyzerController : Controller
    {
        [HttpGet]
        public IActionResult Analyze()
        {
            return RedirectToAction("Index", "Home");
        }

        // 🔹 CHANGED: IActionResult → async Task<IActionResult>
        [HttpPost]
        public async Task<IActionResult> Analyze(string inputText)
        {
            if (string.IsNullOrWhiteSpace(inputText))
            {
                return RedirectToAction("Index", "Home");
            }

            if (inputText.Length > 280)
            {
                return RedirectToAction("Index", "Home");
            }

            var engine = new MHTMEngine();

            // 🔹 CHANGED: Process → await ProcessAsync
            var result = await engine.ProcessAsync(inputText);

            return View("~/Views/Home/Result.cshtml", result);
        }
    }
}
