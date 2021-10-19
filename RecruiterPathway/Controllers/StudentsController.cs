using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RecruiterPathway.Models;
using RecruiterPathway.Authentication;
using System;
using RecruiterPathway.Repository;
using System.Collections.Generic;

namespace RecruiterPathway.Controllers
{
    //Force authorization to view ANYTHING on the student controller
    [Authorize]
    public class StudentsController : Controller
    {
        private IStudentRepository repository;

        public StudentsController(IStudentRepository repository)
        {
            this.repository = repository;
        }


        // GET: Students
        public async Task<IActionResult> Index(string studentDegree, string searchFirstName, string searchLastName, DateTime gradDateStart, DateTime gradDateEnd)
        {
            IEnumerable<Student> students = await repository.GetAll();

            //TODO: All of this filter code is broken, will need to fix in the Repositories
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
                Degrees = repository.GetStudentDegrees(),
                Students = (System.Collections.Generic.List<Student>)students
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
            var student = await repository.GetById((int)id);
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
                var students = await repository.Get(st => st.firstName.Contains(student.firstName) && st.lastName.Contains(student.lastName));
                if (students == null)
                {
                    await repository.Insert(student);
                    repository.Save();
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

            var student = await repository.GetById((int)id);
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
        public IActionResult Edit(int id, [Bind("Id,firstName,lastName")] Student student)
        {
            if (id != student.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                repository.Update(student);
                repository.Save();
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

            var student = await repository.GetById((int)id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            repository.Delete(id);
            repository.Save();
            return RedirectToAction(nameof(Index));
        }
        protected override void Dispose(bool disposing)
        {
            repository.Dispose();
            base.Dispose(disposing);
        }
    }
}
