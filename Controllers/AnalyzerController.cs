

using Microsoft.AspNetCore.Mvc;
using TM_MULTIHEAD_PHISHING_DETECTOR.Models;

namespace TM_MULTIHEAD_PHISHING_DETECTOR.Controllers

{
    public class AnalyzerController : Controller
    {
        [HttpPost]
        public IActionResult Analyze(string postText)
        {
            if (string.IsNullOrWhiteSpace(postText))
            {
                return RedirectToAction("Index");
            }
                
            var engine = new MHTMEngine();
            //var result = engine.Process(postText);

            //return View("Result", result);
        }
    }
}
