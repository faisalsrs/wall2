using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using theWall.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace theWall.Controllers
{
    public class HomeController : Controller

    {
        private Context dbContext;
        public HomeController(Context context)
        {
            dbContext = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("User/new")]
        public IActionResult registration(User newUser)
        {
            if (ModelState.IsValid)
            {
                if (dbContext.Users.FirstOrDefault(q => q.email == newUser.email) != null)
                {
                    ModelState.AddModelError("email", "Please log in");
                    return View("Index");
                }
                PasswordHasher<User> hasher = new PasswordHasher<User>();
                newUser.password = hasher.HashPassword(newUser, newUser.password);
                dbContext.Add(newUser);
                dbContext.SaveChanges();
                HttpContext.Session.SetInt32("userId", newUser.UserId);
                return RedirectToAction("wall");
            }
            return View("Index");
        }
        [HttpPost("User/login")]
        public IActionResult login(Login userLogin)
        {
            if (ModelState.IsValid)
            {
                User isInDb = dbContext.Users.FirstOrDefault(q => q.email == userLogin.lemail);
                if (isInDb == null)
                {
                    ModelState.AddModelError("lemail", "Invalid credentails");
                    return View("Login");
                }
                var hasher = new PasswordHasher<Login>();
                var result = hasher.VerifyHashedPassword(userLogin, isInDb.password, userLogin.lpassword);
                if (result == 0)
                {
                    ModelState.AddModelError("lemail", "Invalid credentails");
                    return View("Login");
                }
                HttpContext.Session.SetInt32("userId", isInDb.UserId);
                return RedirectToAction("wall");
            }
            return View("Login");
        }

        [HttpGet("User/wall")]
        public IActionResult wall()
        {
            if (HttpContext.Session.GetInt32("userId") == null)
            {
                return RedirectToAction("Index");
            }
            if (ModelState.IsValid)
            {
                int uid = (int)HttpContext.Session.GetInt32("userId");
                User user = dbContext.Users.FirstOrDefault(q => q.UserId == uid);
                List<Post> allP = dbContext.Posts.Include(q => q.Creator).Include(v => v.Votes).OrderByDescending(q => q.Votes.Count).ToList();
                successModel model = new successModel();
                model.userLogged = user;
                model.allP = allP;
                return View(model);
            }
            return View();
        }

        [HttpPost("Post/new")]
        public IActionResult newPost(successModel newPost)
        {
            if (HttpContext.Session.GetInt32("userId") == null)
            {
                return RedirectToAction("Index");
            }
            if (ModelState.IsValid)
            {
                newPost.post.UserId = (int)HttpContext.Session.GetInt32("userId");
                dbContext.Add(newPost.post);
                dbContext.SaveChanges();
                return RedirectToAction("wall");
            }
            User user = dbContext.Users.FirstOrDefault(q => q.UserId == (int)HttpContext.Session.GetInt32("userId"));
            ViewBag.user = user;
            TempData["errors"] = "Post must be more than 5 characters.";
            return RedirectToAction("wall");
        }


        [HttpPost("Post/delete/{pId}")]
        public IActionResult delete(int pId)
        {
            if (HttpContext.Session.GetInt32("userId") == null)
            {
                return RedirectToAction("Index");
            }
            Post postToDelete = dbContext.Posts.FirstOrDefault(q => q.PostId == pId);
            dbContext.Remove(postToDelete);
            dbContext.SaveChanges();
            return RedirectToAction("wall");
        }
        [HttpGet("OnePost/{PostId}")]
        public IActionResult OnePost(int PostId)
        {
            if (HttpContext.Session.GetInt32("userId") == null)
            {
                return RedirectToAction("Index");
            }
            Post onepost = dbContext.Posts.Include(w => w.Votes).ThenInclude(r => r.UserVoted).
            Include(p => p.Creator).
            FirstOrDefault(w => w.PostId == PostId);

            //take one event and include all the information from the JOIN list and in that information I also want user information. 
            return View(onepost);
        }
        [HttpGet("OneUser/{UserId}")]
        public IActionResult OneUser(int UserId)
        {
            if (HttpContext.Session.GetInt32("userId") == null)
            {
                return RedirectToAction("Index");
            }
            User oneuser = dbContext.Users.Include(w => w.Votes).ThenInclude(r => r.UserVoted).Include(p => p.postsCreated).FirstOrDefault(w => w.UserId == UserId);

            //take one event and include all the information from the JOIN list and in that information I also want user information. 
            return View(oneuser);
        }



        [HttpPost("Post/vote")]
        public IActionResult Vote(bool vote, int pId)
        {
            if (HttpContext.Session.GetInt32("userId") == null)
            {
                return RedirectToAction("Index");
            }
            Vote isVoted = dbContext.Votes.Where(p => p.PostId == pId).FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("userId")); //checking if there is a vote on a post by user logged in
            if (isVoted == null)
            { // of there is no vote on the post by user, create one
                Vote newVote = new Vote();
                newVote.PostId = pId;
                newVote.IsUpvote = vote;
                newVote.UserId = (int)HttpContext.Session.GetInt32("userId");
                dbContext.Add(newVote);
                dbContext.SaveChanges();
                return RedirectToAction("wall");
            }
            if (isVoted.IsUpvote == vote)
            { // if clicking on the same button again
                dbContext.Remove(isVoted);
            }
            else
            { // clicking on the other button
                isVoted.IsUpvote = vote;
            }
            dbContext.SaveChanges();
            return RedirectToAction("wall");
        }

        [HttpGet("Home/logout")]
        public IActionResult logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
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
    }
}
