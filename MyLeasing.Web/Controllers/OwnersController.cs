﻿namespace MyLeasing.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using MyLeasing.Web.Data;
    using MyLeasing.Web.Data.Entities;
    using MyLeasing.Web.Helpers;
    using MyLeasing.Web.Models;

    [Authorize(Roles = "Manager")]
    public class OwnersController : Controller
    {
        private readonly DataContext _dataContext;

        private readonly IUserHelper _userHelper;

        public OwnersController(
            DataContext context,
            IUserHelper userHelper,)
        {
            _dataContext = context;

            _userHelper = userHelper;
        }

        public async Task<IActionResult> AddProperty(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var owner = await _dataContext.Owners.FindAsync(id.Value);
            if (owner == null)
            {
                return NotFound();
            }

            var view = new PropertyViewModel
            {
                OwnerId = owner.Id,
                PropertyTypes = GetComboPropertyTypes()
            };

            return View(view);
        }

        [HttpPost]
        public async Task<IActionResult> AddProperty(PropertyViewModel view)
        {
            if (ModelState.IsValid)
            {
                var property = await ToPropertyAsync(view);

                _dataContext.Properties.Add(property);

                await _dataContext.SaveChangesAsync();

                return RedirectToAction($"{nameof(Details)}/{view.OwnerId}");
            }

            return View(view);
        }

        private async Task<Property> ToPropertyAsync(PropertyViewModel view)
        {
            return new Property
            {
                Address = view.Address,
                HasParkingLot = view.HasParkingLot,
                IsAvailable = view.IsAvailable,
                Neighborhood = view.Neighborhood,
                Price = view.Price,
                Rooms = view.Rooms,
                SquareMeters = view.SquareMeters,
                Stratum = view.Stratum,
                Owner = 
                await _dataContext.Owners.
                FindAsync(view.OwnerId),
                PropertyType = 
                await _dataContext.PropertyTypes.
                FindAsync(view.PropertyTypeId),
                Remarks = view.Remarks
            };
        }


        private IEnumerable<SelectListItem> GetComboPropertyTypes()
        {
            var list = _dataContext.PropertyTypes.Select(p => new SelectListItem
            {
                Text = p.Name,
                Value = p.Id.ToString()
            }).OrderBy(p => p.Text).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select a property type...)",
                Value = "0"
            });

            return list;

        }


        // GET: Owners
        public IActionResult Index()
        {
            return View(_dataContext.Owners
                .Include(ow => ow.User)
                .Include(ow => ow.Properties)
                .Include(ow => ow.Contracts));
        }


        // GET: Owners/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var owner = await _dataContext.Owners
                .Include(ow => ow.User)
                .Include(ow => ow.Properties)
                .ThenInclude(pt => pt.PropertyType)
                .Include(ow => ow.Properties)
                .ThenInclude(pi => pi.PropertyImages)
                .Include(ow => ow.Contracts)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (owner == null)
            {
                return NotFound();
            }

            return View(owner);
        }

        // GET: Owners/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Owners/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddUserViewModel view)
        {
            if (ModelState.IsValid)
            {
                var user = await AddUser(view);

                if (user == null)
                {
                    ModelState.
                        AddModelError(
                        string.Empty, 
                        "This email is already used.");

                    return View(view);
                }

                var owner = new Owner
                {
                    Properties = new List<Property>(),
                    Contracts = new List<Contract>(),
                    User = user,
                };

                _dataContext.Owners.Add(owner);

                await _dataContext.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(view);
        }

        private async Task<User> AddUser(AddUserViewModel view)
        {
            var user = new User
            {
                Address = view.Address,
                Document = view.Document,
                Email = view.Username,
                FirstName = view.FirstName,
                LastName = view.LastName,
                PhoneNumber = view.PhoneNumber,
                UserName = view.Username
            };

            var result = await _userHelper.AddUserAsync(user, view.Password);

            if (result != IdentityResult.Success)
            {
                return null;
            }

            var newUser = await _userHelper.GetUserByEmailAsync(view.Username);

            await _userHelper.AddUserToRoleAsync(newUser, "Owner");

            return newUser;
        }


        // GET: Owners/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var owner = await _dataContext.Owners.FindAsync(id);
            if (owner == null)
            {
                return NotFound();
            }
            return View(owner);
        }

        // POST: Owners/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id")] Owner owner)
        {
            if (id != owner.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _dataContext.Update(owner);
                    await _dataContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OwnerExists(owner.Id))
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
            return View(owner);
        }

        // GET: Owners/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var owner = await _dataContext.Owners
                .FirstOrDefaultAsync(m => m.Id == id);
            if (owner == null)
            {
                return NotFound();
            }

            return View(owner);
        }

        // POST: Owners/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var owner = await _dataContext.Owners.FindAsync(id);
            _dataContext.Owners.Remove(owner);
            await _dataContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OwnerExists(int id)
        {
            return _dataContext.Owners.Any(e => e.Id == id);
        }
    }
}
