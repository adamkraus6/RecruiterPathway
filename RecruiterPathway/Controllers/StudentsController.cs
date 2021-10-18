using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruiterPathway.Data;
using RecruiterPathway.Models;
using RecruiterPathway.Authentication;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace RecruiterPathway.Controllers
{
    //Force authorization to view ANYTHING on the student controller
    [Authorize]
    public class StudentsController : Controller
    {
        private readonly DatabaseContext _context;

        public StudentsController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: Students
        public async Task<IActionResult> Index(string studentDegree, string searchFirstName, string searchLastName, DateTime gradDateStart, DateTime gradDateEnd)
        {
            // Use LING to get list of genres
            IQueryable<string> degreeQuery = from s in _context.Student
                                             orderby s.degree
                                             select s.degree;

            var students = from s in _context.Student
                           select s;

            if (!string.IsNullOrEmpty(searchFirstName))
            {
                students = students.Where(st => st.firstName.Contains(searchFirstName));
            }

            if (!string.IsNullOrEmpty(searchLastName))
            {
                students = students.Where(st => st.lastName.Contains(searchLastName));
            }

            if (!string.IsNullOrEmpty(studentDegree))
            {
                students = students.Where(st => st.degree == studentDegree);
            }

            if(DateTime.MinValue != gradDateStart && DateTime.MinValue != gradDateEnd)
            {
                students = students.Where(st => gradDateStart.CompareTo(st.gradDate) < 0 && gradDateEnd.CompareTo(st.gradDate) >= 0);
            }

            var studentDegreeVM = new StudentDegreeViewModel
            {
                Degrees = new SelectList(await degreeQuery.Distinct().ToListAsync()),
                Students = await students.ToListAsync()
            };

            return View(studentDegreeVM);
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        public IActionResult Create(string errormessage)
        {
            if (errormessage != null)
            {
                //Convert the fake newlines to true newlines
                string error = "";
                foreach (var chr in errormessage)
                {
                    if (chr == '|')
                    {
                        error += '\n';
                    }
                    else
                    {
                        error += chr;
                    }
                }
                ViewData["error-message"] = error;
            }
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,firstName,lastName")] Student student)
        {
            if (ModelState.IsValid)
            {
                var students = from s in _context.Student
                               select s;
                students = students.Where(st => st.firstName.Contains(student.firstName));
                students = students.Where(st => st.lastName.Contains(student.lastName));
                if (students == null)
                {
                    _context.Add(student);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    return Redirect("~/Students/Create/?errormessage=Already Exists");

                }
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }


        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,firstName,lastName")] Student student)
        {
            if (id != student.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Student.FindAsync(id);
            _context.Student.Remove(student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
            return _context.Student.Any(e => e.Id == id);
        }
    }
}
