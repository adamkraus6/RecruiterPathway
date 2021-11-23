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
        private readonly IStudentRepository _studentRepository;
        private readonly IRecruiterRepository _recruiterRepo;
        private int i = 1;
        public StudentsController(IStudentRepository studentRepository, IRecruiterRepository recruiterRepo)
        {
            this._studentRepository = studentRepository;
            this._recruiterRepo = recruiterRepo;
        }

        // GET: Students
        public async Task<IActionResult> Index(StudentViewModel studentViewModel)
        {
            IEnumerable<Student> students = await _studentRepository.GetAll(studentViewModel);

            var studentVM = new StudentViewModel
            {
                Degrees = _studentRepository.GetStudentDegrees(),
                Students = students.ToList(),
                ListView = studentViewModel.ListView
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
            var student = await _studentRepository.GetById(id);
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
                var students = await _studentRepository.Get(st => st.FirstName.CompareTo(studentViewModel.Student.FirstName) == 0 && st.LastName.CompareTo(studentViewModel.Student.LastName) == 0);
                if (!students.Any())
                {
                    var getId = await _studentRepository.GetById(i.ToString());
                    while (getId != null)
                    {
                        i++;
                        getId = await _studentRepository.GetById(i.ToString());
                    }
                    studentViewModel.Student.Id = i.ToString();
                    await _studentRepository.Insert(studentViewModel.Student);
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

            var student = await _studentRepository.GetById(id);
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
        public async Task<IActionResult> Edit(StudentViewModel studentViewModel)
        {
            if (ModelState.IsValid)
            {
                await _studentRepository.Update(studentViewModel.Student);
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

            var student = await _studentRepository.GetById(id);
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
            await _studentRepository.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        // GET: Students/AddComment/5
        public async Task<IActionResult> AddComment(string id)
        {
            var studentVM = new StudentViewModel
            {
                Student = await _studentRepository.GetById(id)
            };

            return View(studentVM);
        }

        // POST: Students/AddComment/5
        [HttpPost, ActionName("AddComment")]
        public async Task<IActionResult> AddComment(string id, StudentViewModel studentViewModel)
        {
            var student = await _studentRepository.GetById(id);
            if(string.IsNullOrEmpty(studentViewModel.AddCommentText))
            {
                // empty comment, error message TODO
            }
            var comment = new Comment(await _recruiterRepo.GetSignedInRecruiter(HttpContext.User, true), student, studentViewModel.AddCommentText);
            var studentVM = new StudentViewModel
            {
                Student = student,
                Comment = comment
            };

            await _studentRepository.AddComment(studentVM);

            return RedirectToAction(nameof(Details), new { id });
        }

        // Get: Students/AddToWatchList/5
        public async Task<IActionResult> AddToWatchList(string id)
        {
            var studentVM = new StudentViewModel
            {
                Student = await _studentRepository.GetById(id)
            };

            return View(studentVM);
        }

        // POST: Students/AddToWatchList/5
        [HttpPost, ActionName("AddToWatchList")]
        public async Task<IActionResult> AddToWatchList(string id, StudentViewModel studentViewModel)
        {
            var student = await _studentRepository.GetById(id);
            var recruiter = await _recruiterRepo.GetSignedInRecruiter(HttpContext.User, true);
            await _recruiterRepo.AddWatch(recruiter, student);

            return Redirect("~/Recruiters/Profile");
        }

        // Get: Students/AddPipelineStatus/5
        public async Task<IActionResult> AddPipelineStatus(string id)
        {
            var studentVM = new StudentViewModel
            {
                Student = await _studentRepository.GetById(id)
            };

            return View(studentVM);
        }

        // POST: Students/AddPipelineStatus/5
        [HttpPost, ActionName("AddPipelineStatus")]
        public async Task<IActionResult> AddPipelineStatus(string id, StudentViewModel studentViewModel)
        {
            var student = await _studentRepository.GetById(id);
            var recruiter = await _recruiterRepo.GetSignedInRecruiter(HttpContext.User, true);
            await _recruiterRepo.SetPipelineStatus(recruiter, student, studentViewModel.PipelineStatus);

            return Redirect("~/Recruiters/Profile");
        }

        protected override void Dispose(bool disposing)
        {
            _studentRepository.Dispose();
            base.Dispose(disposing);
        }
    }
}
