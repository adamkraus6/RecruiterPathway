using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RecruiterPathway.Models;
using RecruiterPathway.Authentication;
using System;
using RecruiterPathway.Repository;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Rendering;
using RecruiterPathway.ViewModels;

namespace RecruiterPathway.Controllers
{
    //Force authorization to view ANYTHING on the student controller
    [Authorize]
    public class StudentsController : Controller
    {
        private readonly IStudentRepository repository;
        private readonly IRecruiterRepository recruiterRepo;
        private int i = 1;
        private readonly string[] sortOptions = { "First Name", "Last Name", "Degree", "Graduation Date" };
        public StudentsController(IStudentRepository repository, IRecruiterRepository recruiterRepo)
        {
            this.repository = repository;
            this.recruiterRepo = recruiterRepo;
        }

        // GET: Students
        public async Task<IActionResult> Index(StudentViewModel studentViewModel)
        {
            IEnumerable<Student> students = await repository.GetAll();

            if (!string.IsNullOrEmpty(studentViewModel.SearchFirstName))
            {
                students = students.Where(st => st.firstName.Contains(studentViewModel.SearchFirstName));
            }

            if (!string.IsNullOrEmpty(studentViewModel.SearchLastName))
            {
                students = students.Where(st => st.lastName.Contains(studentViewModel.SearchLastName));
            }

            if (!string.IsNullOrEmpty(studentViewModel.StudentDegree))
            {
                students = students.Where(st => st.degree == studentViewModel.StudentDegree);
            }

            if (DateTime.MinValue != studentViewModel.GradDateStart && DateTime.MinValue != studentViewModel.GradDateEnd)
            {
                students = students.Where(st => studentViewModel.GradDateStart.CompareTo(st.gradDate) < 0 && studentViewModel.GradDateEnd.CompareTo(st.gradDate) >= 0);
            }

            switch (studentViewModel.SortBy)
            {
                default:
                    break;
                case "First Name":
                    students = students.OrderBy(st => st.firstName);
                    break;
                case "Last Name":
                    students = students.OrderBy(st => st.lastName);
                    break;
                case "Degree":
                    students = students.OrderBy(st => st.degree);
                    break;
                case "Graduation Date":
                    students = students.OrderBy(st => st.gradDate);
                    break;
            }

            var studentVM = new StudentViewModel
            {
                Degrees = repository.GetStudentDegrees(),
                Students = students.ToList(),
                ListView = studentViewModel.ListView,
                SortOptions = new SelectList(sortOptions)
            };

            return View(studentVM);
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var student = await repository.GetById(id);
            if (student == null)
            {
                return NotFound();
            }

            var studentVM = new StudentViewModel
            {
                Student = student
            };

            return View(studentVM);
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
        public async Task<IActionResult> Create(StudentViewModel studentViewModel)
        {
            if (ModelState.IsValid)
            {
                var students = await repository.Get(st => st.firstName.CompareTo(studentViewModel.Student.firstName) == 0 && st.lastName.CompareTo(studentViewModel.Student.lastName) == 0);
                if (!students.Any())
                {
                    var getId = await repository.GetById(i.ToString());
                    while (getId != null)
                    {
                        i++;
                        getId = await repository.GetById(i.ToString());
                    }
                    studentViewModel.Student.Id = i.ToString();
                    await repository.Insert(studentViewModel.Student);
                }
                else
                {
                    return Redirect("~/Students/Create/?errormessage=Already Exists");

                }
                return RedirectToAction(nameof(Index));
            }

            var studentVM = new StudentViewModel
            {
                Student = studentViewModel.Student
            };

            return View(studentVM);
        }


        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await repository.GetById(id);
            if (student == null)
            {
                return NotFound();
            }

            var studentVM = new StudentViewModel
            {
                Student = student
            };

            return View(studentVM);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, StudentViewModel studentViewModel)
        {
            if (!id.Equals(studentViewModel.Student.Id))
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await repository.Update(studentViewModel.Student);
                repository.Save();
                return RedirectToAction(nameof(Index));
            }

            var studentVM = new StudentViewModel
            {
                Student = studentViewModel.Student
            };

            return View(studentVM);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await repository.GetById(id);
            if (student == null)
            {
                return NotFound();
            }

            var studentVM = new StudentViewModel
            {
                Student = student
            };

            return View(studentVM);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await repository.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        // GET: Students/AddComment/5
        public async Task<IActionResult> AddComment(string id)
        {
            var studentVM = new StudentViewModel
            {
                Student = await repository.GetById(id)
            };

            return View(studentVM);
        }

        // POST: Students/AddComment/5
        [HttpPost, ActionName("AddComment")]
        public async Task<IActionResult> AddComment(string id, StudentViewModel studentViewModel)
        {
            var student = await repository.GetById(id);
            if(string.IsNullOrEmpty(studentViewModel.AddCommentText))
            {

            }
            var comment = new Comment(await recruiterRepo.GetSignedInRecruiter(HttpContext.User), student, studentViewModel.AddCommentText);
            var studentVM = new StudentViewModel
            {
                Student = student,
                Comment = comment
            };

            await repository.AddComment(studentVM);

            return Redirect("~/Students/Details/" + id);
        }

        protected override void Dispose(bool disposing)
        {
            repository.Dispose();
            base.Dispose(disposing);
        }
    }
}
