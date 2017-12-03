﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Newtonsoft.Json;
using ParrotWings.API;
using ParrotWings.API.Models;

namespace ParrotWings.API.Controllers
{
    [Authorize]
    [RoutePrefix("api/Users")]
    public class UsersController : ApiController
    {
        private ParrotWingsContext db = new ParrotWingsContext();

        [Route("GetUsers")]
        public string GetUsers(string term)
        {
            try
            {
                var currentUserName = User.Identity.Name;

                var users = db.Users
                    .Where(el => el.Name.StartsWith(term)
                                 && el.Name != currentUserName)
                    .Select(el => new
                    {
                        el.Id,
                        el.Name
                    })
                    .ToList();

                return JsonConvert.SerializeObject(users);
            }
            catch (Exception)
            {
                return "Server Error. Contact your administrator.";
            }
        }

        [Route("GetName")]
        public string GetName()
        {
            try
            {
                var currentUserName = User.Identity.Name;

                return currentUserName;
            }
            catch (Exception)
            {
                return "Server Error. Contact your administrator.";
            }
        }

        [Route("GetBalance")]
        public string GetBalance()
        {
            try
            {
                var currentUserName = User.Identity.Name;

                var user = db.Users
                    .FirstOrDefault(el => el.Name == currentUserName);

                return user.Balance.ToString();
            }
            catch (Exception)
            {
                return "Server Error. Contact your administrator.";
            }
        }
        // GET: api/Users
        public IQueryable<User> GetUsers()
        {
            return db.Users;
        }

        // GET: api/Users/5
        [ResponseType(typeof(User))]
        public IHttpActionResult GetUser(int id)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // PUT: api/Users/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutUser(int id, User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user.Id)
            {
                return BadRequest();
            }

            db.Entry(user).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Users
        [ResponseType(typeof(User))]
        public IHttpActionResult PostUser(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Users.Add(user);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = user.Id }, user);
        }

        // DELETE: api/Users/5
        [ResponseType(typeof(User))]
        public IHttpActionResult DeleteUser(int id)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            db.Users.Remove(user);
            db.SaveChanges();

            return Ok(user);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserExists(int id)
        {
            return db.Users.Count(e => e.Id == id) > 0;
        }
    }
}