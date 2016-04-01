using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MongoDB.Driver;
using M101DotNet.WebApp.Models;
using M101DotNet.WebApp.Models.Home;
using MongoDB.Bson;
using System.Linq.Expressions;

namespace M101DotNet.WebApp.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            var blogContext = new BlogContext();
            // XXX WORK HERE
            // find the most recent 10 posts and order them
            // from newest to oldest

            var model = new IndexModel
            {
                RecentPosts = await blogContext.Posts.Find(p => true).SortByDescending(p => p.CreatedAtUtc).Limit(10).ToListAsync()
            };

            return View(model);
        }

        [HttpGet]
        public ActionResult NewPost()
        {
            return View(new NewPostModel());
        }

        [HttpPost]
        public async Task<ActionResult> NewPost(NewPostModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var blogContext = new BlogContext();
            // XXX WORK HERE
            // Insert the post into the posts collection
            var post = new Post()
            {
                Tags = model.Tags.Split(',').ToList(),
                Content = model.Content,
                Title = model.Title,
                Author = User.Identity.Name,
                CreatedAtUtc = DateTime.Now.ToUniversalTime(),
                Comments = new List<Comment>()
            };
            await blogContext.Posts.InsertOneAsync(post);

            return RedirectToAction("Post", new { id = post.Id });
        }

        [HttpGet]
        public async Task<ActionResult> Post(string id)
        {
            var blogContext = new BlogContext();

            // XXX WORK HERE
            // Find the post with the given identifier
            var post = await blogContext.Posts.Find(p => p.Id == new ObjectId(id)).FirstOrDefaultAsync();

            if (post == null)
            {
                return RedirectToAction("Index");
            }

            var model = new PostModel
            {
                Post = post
            };

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Posts(string tag = null)
        {
            var blogContext = new BlogContext();

            // XXX WORK HERE
            // Find all the posts with the given tag if it exists.
            // Otherwise, return all the posts.
            // Each of these results should be in descending order.
            var query = tag == null ?
                blogContext.Posts.Find(p => true) :
                blogContext.Posts.Find(p => p.Tags.Contains(tag));


            var posts = await query.SortByDescending(p => p.CreatedAtUtc).ToListAsync();

            return View(posts);
        }

        [HttpPost]
        public async Task<ActionResult> NewComment(NewCommentModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Post", new { id = model.PostId });
            }

            var blogContext = new BlogContext();

            var comment = new Comment()
            {
                Author = User.Identity.Name,
                Content = model.Content,
                CreatedAtUtc = DateTime.Now.ToUniversalTime()
            };

            var result = await blogContext.Posts.UpdateOneAsync(p => p.Id == new ObjectId(model.PostId), Builders<Post>.Update.Push(p => p.Comments, comment));

            // XXX WORK HERE
            // add a comment to the post identified by model.PostId.
            // you can get the author from "this.User.Identity.Name"

            return RedirectToAction("Post", new { id = model.PostId });
        }
    }
}